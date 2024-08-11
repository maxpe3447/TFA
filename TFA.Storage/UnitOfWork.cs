using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using TFA.Domain;

namespace TFA.Storage;

class UnitOfWork(IServiceProvider serviceProvider) : IUnitOfWork
{
    public async Task<IUnitOfWorkScope> StartScope(CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        return new UnitOfWorkScope(scope, transaction);
    }
}

internal class UnitOfWorkScope(
    IServiceScope scope,
    IDbContextTransaction dbTransaction) : IUnitOfWorkScope
{
    public Task Commit(CancellationToken cancellationToken) => dbTransaction.CommitAsync(cancellationToken);

    public TStorage GetStorage<TStorage>() where TStorage : IStorage =>
        scope.ServiceProvider.GetRequiredService<TStorage>();

    public async ValueTask DisposeAsync()
    {
        if (scope is IAsyncDisposable scopeDisposable)
        {
            await scopeDisposable.DisposeAsync();
        }
        else
        {
            scope.Dispose();
        }
        await dbTransaction.DisposeAsync();
    }
}