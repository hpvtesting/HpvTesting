using AutoMapper;
using HPVTesting.Business.ViewModels;
using HPVTesting.Business.ViewModels.Account;
using HPVTesting.Domain.Models;
using HPVTesting.Interfaces.Services;
using HPVTesting.UoW;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HPVTesting.Services
{
    public class UsersService : ServiceBase, IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersService(IUnitOfWork unitOfWork, IMapper _mapper, UserManager<ApplicationUser> userManager) : base(unitOfWork, _mapper)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddUser(UserModel userModel, int EmailVerificationCode, bool isAdmin, bool isExternal = false)
        {
            var result = new IdentityResult();
            if (!isExternal)
            {
                var applicationUser = new ApplicationUser { Email = userModel.Email, UserName = userModel.Email };
                result = await _userManager.CreateAsync(applicationUser, userModel.Password);
            }
            else
            {
                var applicationUser = new ApplicationUser { Email = userModel.Email, UserName = userModel.Email };
                result = await _userManager.CreateAsync(applicationUser);
            }

            if (result.Succeeded)
            {
                var aspNetUser = await _userManager.FindByEmailAsync(userModel.Email);

                if (isExternal)
                {
                    aspNetUser.EmailConfirmed = true;
                    await _userManager.UpdateAsync(aspNetUser);
                }

                string role = "User";

                if (isAdmin)
                {
                    role = "Admin";
                }

                var roleResult = await _userManager.AddToRoleAsync(aspNetUser, role);

                if (roleResult.Succeeded)
                {
                    var user = mapper.Map<User>(userModel);

                    user.Id = Guid.NewGuid();
                    user.AspNetUserId = Convert.ToString(aspNetUser.Id.ToString());
                    user.CreatedAt = System.DateTime.Now;
                    user.UpdatedAt = System.DateTime.Now;
                    user.CreatedBy = Guid.Empty;
                    user.IsDelete = false;
                    user.UpdatedBy = Guid.Empty;
                    user.Name = userModel.FirstName + " " + userModel.LastName;

                    user.EmailVerificationCode = EmailVerificationCode;

                    await unitOfWork.UserRepository.AddAsync(user);
                }
                else
                {
                    await _userManager.DeleteAsync(aspNetUser);

                    return mapper.Map<IdentityResult>(roleResult);
                }
            }

            return mapper.Map<IdentityResult>(result);
        }

        public async Task<User> GetCoreUserAsync(Guid id)
        {
            var user = await unitOfWork.UserRepository.GetAsync(id);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<UserViewModel> GetByAspnetUserIdAsync(string id)
        {
            var user = unitOfWork.UserRepository.GetUserDetails(id);
            if (user != null)
            {
                var userViewModel = mapper.Map<UserViewModel>(user);

                var userRoles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(id));

                userViewModel.UserRole = string.Join(',', userRoles);

                return userViewModel;
            }

            return null;
        }

        public async Task<bool> AddUserSocalConnection(UserSocialConnection userSocialConnection)
        {
            userSocialConnection.Id = Guid.NewGuid();
            userSocialConnection.CreatedAt = System.DateTime.Now;
            userSocialConnection.UpdatedAt = System.DateTime.Now;
            userSocialConnection.CreatedBy = Guid.Empty;
            userSocialConnection.UpdatedBy = Guid.Empty;
            var responce = await unitOfWork.UserSocialConnectionRepository.AddAsync(userSocialConnection);
            await unitOfWork.UserSocialConnectionRepository.SaveAsync();
            if (responce != null)
            {
                return true;
            }
            return false;
        }


        public async Task<bool> UpdateUserVerificationCode(User user)
        {
            var userFriends = unitOfWork.UserRepository.FindByAsync(x => x.AspNetUserId == user.AspNetUserId).Result.Where(x => !x.IsDelete);
            if (userFriends.Any())
            {
                var userDetails = userFriends.FirstOrDefault();
                var aspNetUser = await _userManager.FindByEmailAsync(user.AspNetUser.Email);
                aspNetUser.PasswordHash = user.AspNetUser.PasswordHash;
                userDetails.UpdatedAt = DateTime.Now;

                await unitOfWork.UserRepository.UpdateAsync(userDetails);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(true); ;
        }
       
        public async Task<bool> UpdateUser(UserModel userModel)
        {
            var user = await unitOfWork.UserRepository.GetAsync(userModel.Id);
            user = mapper.Map(userModel, user);

            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    //await DocumentHelper.DeleteFilesToAWS(user.ProfileImage);
                }
                if (!string.IsNullOrEmpty(user.ThumbnailProfileImage))
                {
                    //await DocumentHelper.DeleteFilesToAWS(user.ThumbnailProfileImage);
                }
                user.ProfileImage = userModel.ProfileImage;
                user.ThumbnailProfileImage = userModel.ThumbnailProfileImage;
                user.UpdatedAt = DateTime.UtcNow;
                user.IsDelete = false;
                user.UpdatedBy = Guid.Empty;
                user.Name = userModel.FirstName + " " + userModel.LastName;

                //MAP other fields
                await unitOfWork.UserRepository.UpdateAsync(user);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<UserViewModel> GetAsync(Guid id)
        {
            var user = await unitOfWork.UserRepository.GetAsync(id);
            if (user == null)
            {
                return null;
            }

            var userViewModel = mapper.Map<UserViewModel>(user);

            var userRoles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.AspNetUserId));

            userViewModel.UserRole = string.Join(',', userRoles);

            return userViewModel;
        }

        public async Task<bool> VerifyEmail(EmailVerifyModel emailVerifyModel)
        {
            var user = await unitOfWork.UserRepository.GetAsync(emailVerifyModel.UserId);
            if (user != null)
            {
                if (user.EmailVerificationCode == emailVerifyModel.Code)
                {
                    var aspNetUser = await _userManager.FindByIdAsync(user.AspNetUserId);
                    if (aspNetUser == null)
                    {
                        return await Task.FromResult(false);
                    }

                    aspNetUser.EmailConfirmed = true;
                    await _userManager.UpdateAsync(aspNetUser);

                    return await Task.FromResult(true);
                }
            }
            return await Task.FromResult(false);
        }
    }
}