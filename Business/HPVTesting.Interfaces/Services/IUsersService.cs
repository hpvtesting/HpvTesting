using HPVTesting.Business.ViewModels;
using HPVTesting.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace HPVTesting.Interfaces.Services
{
    public interface IUsersService : IBaseService<User>
    {
        Task<UserViewModel> GetAsync(Guid id);
        Task<bool> UpdateUser(UserModel userModel);
        Task<bool> VerifyEmail(EmailVerifyModel emailVerifyModel);
        Task<IdentityResult> AddUser(UserModel applicationUser, int EmailVerificationCode, bool isAdmin, bool isExternal = false);

        Task<UserViewModel> GetByAspnetUserIdAsync(string id);
        Task<User> GetCoreUserAsync(Guid id);
        Task<bool> AddUserSocalConnection(UserSocialConnection userSocialConnection);
    }
}