using HPVTesting.Interfaces.Repository;
using System;

namespace HPVTesting.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }

        IUserSocialConnectionRepository UserSocialConnectionRepository { get; }
    }
}