namespace DataAccessLibrary.Data;

public interface IRepository<TEntity, TId> where TEntity : class where TId : struct, IComparable<TId>
{
    IQueryable<TEntity> GetQueryable();
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TId id);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
}