﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using ProductionChain.Contracts.IUnitOfWork;

namespace ProductionChain.DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _db;

    private readonly IServiceProvider _serviceProvider;

    private IDbContextTransaction? _transaction;

    private bool _disposed;

    public UnitOfWork(DbContext db, IServiceProvider serviceProvider)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public T GetRepository<T>() where T : class
    {
        ThrowExceptionIfDisposed();

        return _serviceProvider.GetRequiredService<T>();
    }

    public void BeginTransaction()
    {
        ThrowExceptionIfDisposed();

        if (_transaction != null)
        {
            throw new InvalidOperationException("Транзакция уже создана");
        }

        _transaction = _db.Database.BeginTransaction();
    }

    public void RollbackTransaction()
    {
        ThrowExceptionIfDisposed();

        if (_transaction is not null)
        {
            _transaction.Rollback();
            _transaction = null;
        }
    }

    public async Task SaveAsync()
    {
        ThrowExceptionIfDisposed();

        await _db.SaveChangesAsync();

        if (_transaction is not null)
        {
            _transaction.Commit();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        RollbackTransaction();

        _db.Dispose();
        _disposed = true;
    }

    private void ThrowExceptionIfDisposed()
    {
        if (!_disposed)
        {
            return;
        }

        throw new ObjectDisposedException("Объект уже удалён и больше не доступен.");
    }
}
