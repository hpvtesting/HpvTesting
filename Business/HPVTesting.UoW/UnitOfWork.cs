using HPVTesting.Domain;
using HPVTesting.Interfaces.Repository;
using HPVTesting.Repositories;
using System;

namespace HPVTesting.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HPVTestingContext Context;

        public UnitOfWork(HPVTestingContext context)
        {
            this.Context = context;

            UserRepository = new UserRepository(Context);

            UserSocialConnectionRepository = new UserSocialConnectionRepository(Context);
        }

        public IUserRepository UserRepository { get; }

        public IUserSocialConnectionRepository UserSocialConnectionRepository { get; }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                Context.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}