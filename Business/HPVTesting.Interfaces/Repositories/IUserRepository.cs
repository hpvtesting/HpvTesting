using HPVTesting.Domain.Models;

namespace HPVTesting.Interfaces.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User GetUserDetails(string aspNetUserId);
    }
}