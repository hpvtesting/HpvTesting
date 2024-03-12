using HPVTesting.API.Helpers;
using HPVTesting.Business.Enums;
using HPVTesting.Business.Enums.General;
using HPVTesting.Business.Helpers;
using HPVTesting.Business.Models;
using HPVTesting.Business.ViewModels;
using HPVTesting.Business.ViewModels.Account;
using HPVTesting.Domain;
using HPVTesting.Domain.Models;
using HPVTesting.Interfaces.Background;
using HPVTesting.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace HPVTesting.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : BaseApiController
    {
        protected static new readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IUsersService _usersService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly new RoleManager<IdentityRole> _identityManager;
        private readonly HPVTestingContext _db;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IBackgroundService _backgroundService;


        public UserController(IHttpClientFactory httpClientFactory, HPVTestingContext db, RoleManager<IdentityRole> identityManager, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUsersService usersService, IBackgroundService backgroundService)
        {
            _httpClientFactory = httpClientFactory;
            _usersService = usersService;
            _userManager = userManager;
            _signInManager = signInManager;
            _backgroundService = backgroundService;
            _identityManager = identityManager;
            _db = db;
        }
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<object> Login([FromBody] PersonModel personModel)
        {
            return await GetDataWithMessage(async () =>
            {
                if (personModel != null)
                {
                    if (!string.IsNullOrEmpty(personModel.Token) && string.IsNullOrEmpty(personModel.Name) && string.IsNullOrEmpty(personModel.Password))
                    {
                        var user = await GetLoginResponseDetails(ApiTokenHelper.ValidateToken(personModel.Token)?.FindFirst("Email")?.Value);

                        user.Token = ApiTokenHelper.GenerateJSONWebToken(user.ApplicationUser, user.UserDetail);
                        return Response(user, string.Empty);
                    }

                    var result = await _signInManager.PasswordSignInAsync(personModel.Name, personModel.Password, false, false);
                    if (result.IsNotAllowed)
                    {
                        return Response(await GetLoginResponseDetails(personModel.Name), string.Empty);
                    }

                    if (result.Succeeded)
                    {
                        var user = await GetLoginResponseDetails(personModel.Name);
                        if (user.UserDetail.IsProfileComplete == null)
                        {
                            user.UserDetail.IsProfileComplete = false;
                        }
                        user.Token = ApiTokenHelper.GenerateJSONWebToken(user.ApplicationUser, user.UserDetail);

                        return Response(user, string.Empty);
                    }
                }
                return Response(new LoginResponseModel(), "Email or password invalid".ToString(), DropMessageType.Error);
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ExternalLogin")]
        public async Task<object> ExternalLogin([FromBody] ExternalAuthModel externalAuth)
        {
            return await GetDataWithMessage(async () =>
            {
                if (externalAuth.Provider == "GOOGLE")
                {
                    return await LoginWithGoogle(externalAuth);
                }

                return Response(new LoginResponseModel(), "Somthing Wants Wrong Please Try Again Latter.", DropMessageType.Error);
            });
        }

        private async Task<Tuple<LoginResponseModel, string, DropMessageType>> LoginWithGoogle(ExternalAuthModel externalAuth)
        {
            var payload = await new JwtHandler().VerifyGoogleToken(externalAuth);

            if (payload == null)
                return Response(new LoginResponseModel(), "Invalid External Authentication.", DropMessageType.Error);

            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    var userModel = new UserModel
                    {
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Email = payload.Email
                    };

                    var responce = await _usersService.AddUser(userModel, 0, false, true);
                    if (responce.Succeeded)
                    {
                        var loginResponceModel = await GetLoginResponseDetails(payload.Email);
                        loginResponceModel.Token = ApiTokenHelper.GenerateJSONWebToken(loginResponceModel.ApplicationUser, loginResponceModel.UserDetail);
                        await _userManager.AddLoginAsync(loginResponceModel.ApplicationUser, info);
                        return Response(loginResponceModel, "Login Success", DropMessageType.Success);
                    }
                    else
                    {
                        return Response(new LoginResponseModel(), "Somthing Wants Wrong Please Try Again Latter.", DropMessageType.Error);
                    }
                }
                else
                {
                    var loginResponceModel = await GetLoginResponseDetails(payload.Email);
                    loginResponceModel.Token = ApiTokenHelper.GenerateJSONWebToken(loginResponceModel.ApplicationUser, loginResponceModel.UserDetail);
                    await _usersService.AddUserSocalConnection(new UserSocialConnection { UserId = loginResponceModel.UserDetail.Id, SocialId = info.ProviderKey, Type = (int)SocialMediaType.Google });
                    await _userManager.AddLoginAsync(user, info);
                    return Response(loginResponceModel, "Login Success", DropMessageType.Success);
                }
            }
            else
            {
                var loginResponceModel = await GetLoginResponseDetails(payload.Email);
                loginResponceModel.Token = ApiTokenHelper.GenerateJSONWebToken(loginResponceModel.ApplicationUser, loginResponceModel.UserDetail);
                await _userManager.AddLoginAsync(user, info);
                return Response(loginResponceModel, "Login Success", DropMessageType.Success);
            }
        }

        private async Task<LoginResponseModel> GetLoginResponseDetails(string email)
        {
            var user = new LoginResponseModel
            {
                ApplicationUser = await _userManager.FindByEmailAsync(email)
            };

            var userDetail = await _usersService.GetByAspnetUserIdAsync(user.ApplicationUser.Id);
            user.UserDetail = userDetail;

            return user;
        }

        [HttpPost]
        [Route("ResetPassword")]
        [AllowAnonymous]
        public async Task<object> ResetPassword([FromBody] ForgotPasswordModel forgotPasswordModel)
        {
            return await GetDataWithMessage(async () =>
            {
                var responce = new IdentityResult();
                var data = await _usersService.GetCoreUserAsync(forgotPasswordModel.UserId);
                if (data != null && data.AspNetUser != null)
                {
                    var passwordresetToken = await _userManager.GeneratePasswordResetTokenAsync(data.AspNetUser);
                    responce = await _userManager.ResetPasswordAsync(data.AspNetUser, passwordresetToken, forgotPasswordModel.Password);
                    if (responce.Succeeded)
                    {
                        return Response(responce, "Password Reset Sucessfully");
                    }
                }
                return Response(responce, "Password Reset Faild", DropMessageType.Error);
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<object> Post([FromBody] UserModel userModel)
        {
            return await GetDataWithMessage(async () =>
            {

                if (userModel != null)
                {
                    return (userModel.Id == Guid.Empty) ? await AddAsync(userModel, false) : await UpdateAsync(userModel);
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage);
                return Response(new UserViewModel(), string.Join(",", errors.FirstOrDefault()), DropMessageType.Error);
            });
        }

        [HttpPost]
        [Route("AdminPost")]
        public async Task<object> AdminPost([FromBody] UserModel userModel)
        {
            return await GetDataWithMessage(async () =>
            {
                if (userModel != null)
                {
                    return (userModel.Id == Guid.Empty) ? await AdminAddAsync(userModel, true) : await UpdateAsync(userModel);
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage);
                return Response(new UserViewModel(), string.Join(",", errors.FirstOrDefault()), DropMessageType.Error);
            });
        }

        private async Task<Tuple<UserViewModel, string, DropMessageType>> AdminAddAsync(UserModel userModel, bool isAdmin)
        {
            //Random random = new Random((int)DateTime.Now.Ticks);
            //int EmailVerificationCode = random.Next(10000, 99999);

            var user = await _usersService.AddUser(userModel, 00000, isAdmin);
            if (user.Succeeded)
            {

                var aspNetUser = await _userManager.FindByEmailAsync(userModel.Email);

                var userDetail = await _usersService.GetByAspnetUserIdAsync(aspNetUser.Id);

                var redirectUrl = new AppSettings().WebUrl + "Login/VerfiyAdminnUser?userId=" + aspNetUser.Id;

                //var redirectUrl = new AppSettings().WebUrl + "Login/ResetPassword?userId=" + data.UserDetail.Id;
                //_backgroundService.EnqueueJob<IBackgroundMailerJobs>(m => m.SendForgotPasswordEmail(redirectUrl, data.ApplicationUser.Email));

                //_backgroundService.EnqueueJob<IBackgroundMailerJobs>(m => m.SendVerifyAdminUserEmail(redirectUrl, aspNetUser.Email));

                //_backgroundService.EnqueueJob<IBackgroundMailerJobs>(m => m.SendWelcomeEmail(aspNetUser.UserName, aspNetUser.Email));

                return Response(userDetail, "sucess");
            }
            var data = string.Join(",", user.Errors.Select(x => x.Description));

            return Response(new UserViewModel(), string.Join(",", user.Errors.Select(x => x.Description)), DropMessageType.Error);
        }

        private async Task<Tuple<UserViewModel, string, DropMessageType>> AddAsync(UserModel userModel, bool isAdmin)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            int EmailVerificationCode = random.Next(10000, 99999);

            var user = await _usersService.AddUser(userModel, EmailVerificationCode, isAdmin);
            if (user.Succeeded)
            {
                //_backgroundService.EnqueueJob<IBackgroundMailerJobs>(m => m.SendEmailVerificationCode(EmailVerificationCode, userModel.Email));

                //_backgroundService.EnqueueJob<IBackgroundMailerJobs>(m => m.SendWelcomeEmail(userModel.FirstName, userModel.Email));

                var aspNetUser = await _userManager.FindByEmailAsync(userModel.Email);

                var userDetail = await _usersService.GetByAspnetUserIdAsync(aspNetUser.Id);

                return Response(userDetail, "sucess");
            }
            var data = string.Join(",", user.Errors.Select(x => x.Description));

            return Response(new UserViewModel(), string.Join(",", user.Errors.Select(x => x.Description)), DropMessageType.Error);
        }

        private async Task<Tuple<UserViewModel, string, DropMessageType>> UpdateAsync(UserModel userModel)
        {
            var flag = await _usersService.UpdateUser(userModel);
            if (flag)
            {
                var user = await _usersService.GetAsync(userModel.Id);
                return Response(user, "Update Sucess");
            }
            return Response(new UserViewModel(), "Update Fail", DropMessageType.Error);
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("VerifyEmail")]
        public async Task<object> VerifyEmail([FromBody] EmailVerifyModel userModel)
        {
            return await GetDataWithMessage(async () =>
            {
                if (ModelState.IsValid && userModel != null)
                {
                    if (await _usersService.VerifyEmail(userModel))
                    {
                        return Response(true, "Email verified");
                    }

                    return Response(false, "Entered Code is not Valid", DropMessageType.Error);
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage);
                return Response(false, string.Join(",", errors), DropMessageType.Error);
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ResendVerificationEmail")]
        public async Task<object> ResendVerificationEmail([FromBody] EmailVerifyModel userModel)
        {
            return await GetDataWithMessage(async () =>
            {
                if (ModelState.IsValid && userModel != null)
                {
                    var user = await _usersService.GetCoreUserAsync(userModel.UserId);
                    if (user != null)
                    {
                        var applicationUser = await _userManager.FindByIdAsync(user.AspNetUserId);

                        if (user.EmailVerificationCode == null || user.EmailVerificationCode == 0)
                        {
                            Random random = new Random((int)DateTime.Now.Ticks);
                            user.EmailVerificationCode = random.Next(10000, 99999);
                        }

                        //_backgroundService.EnqueueJob<IBackgroundMailerJobs>(m => m.SendEmailVerificationCode(user.EmailVerificationCode.Value, applicationUser.Email));

                        return Response(true, "Email Sent");
                    }

                    return Response(false, "User not found", DropMessageType.Error);
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage);
                return Response(false, string.Join(",", errors), DropMessageType.Error);
            });
        }
    }
}
