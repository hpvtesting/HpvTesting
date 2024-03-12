using HPVTesting.Business.ViewModels.Account;

namespace HPVTesting.Business.ViewModels
{
    public class LoginResponseModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public UserViewModel UserDetail { get; set; }

        public string Token { get; set; }
    }
}