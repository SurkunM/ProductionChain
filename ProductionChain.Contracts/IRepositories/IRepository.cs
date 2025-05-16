namespace ProductionChain.Contracts.IRepositories;

public interface IRepository
{
}

public interface IRepository<T> : IRepository
{
    Task CreateAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);

    Task SaveAsync();
}
