namespace ProductionChain.Contracts.IRepositories;

public interface IRepository
{
}

public interface IRepository<T> : IRepository
{
    Task CreateAsync(T entity);

    Task<T> CreateAndGetEntityAsync(T entity);

    void Update(T entity);

    void Delete(T entity);

    Task<T?> GetByIdAsync(int id);
}
