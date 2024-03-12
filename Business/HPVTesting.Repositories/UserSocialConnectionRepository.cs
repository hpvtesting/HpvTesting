using HPVTesting.Domain;
using HPVTesting.Domain.Models;
using HPVTesting.Interfaces.Repository;

namespace HPVTesting.Repositories
{
    public class UserSocialConnectionRepository : BaseRepository<UserSocialConnection>, IUserSocialConnectionRepository
    {
        public UserSocialConnectionRepository(HPVTestingContext context) : base(context)
        {
        }
    }
}
