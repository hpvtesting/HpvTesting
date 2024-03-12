using HPVTesting.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HPVTesting.Interfaces.Repository
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        Task<T> GetAsync(Guid id);

        Task<T> AddAsync(T entity);

        Task<int> AddRangeAsync(IEnumerable<T> entities);

        Task<int> DeleteAsync(T entity);

        Task<bool> DeleteAllAsync(IEnumerable<T> entities);

        Task<int> UpdateAsync(T entity);

        Task<int> SaveAsync();
    }
}