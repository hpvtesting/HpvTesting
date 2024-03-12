using HPVTesting.Domain;
using HPVTesting.Domain.Models;
using HPVTesting.Interfaces.Repository;
using System.Linq;

namespace HPVTesting.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(HPVTestingContext context) : base(context)
        {
        }

        public User GetUserDetails(string aspNetUserId) {
            return Context.User.Where(x => x.AspNetUserId == aspNetUserId && !x.IsDelete).FirstOrDefault();
        }
    }
}