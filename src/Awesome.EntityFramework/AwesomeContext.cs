using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Awesome.EntityFramework
{

    public class AwesomeContext<TContext> : IAwesomeContext where TContext : DbContext
    {
        private readonly ILogger<AwesomeContext<TContext>> logger;
        private readonly IAwesomeDbContextFactory<TContext> factory;
        private readonly int maxSaveAttempts;

        public AwesomeContext(ILogger<AwesomeContext<TContext>> logger, IAwesomeDbContextFactory<TContext> factory, int maxSaveAttempts = 10)
        {
            this.logger = logger;
            this.factory = factory;
            this.maxSaveAttempts = maxSaveAttempts;
        }

        public async Task<TResult> Read<TResult>(Func<DbContext, Task<TResult>> fn)
        {
            using (var context = this.factory.Create())
            {
                context.ChangeTracker.LazyLoadingEnabled = false;
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                return await fn(context);
            }
        }

        public async Task<TResult> Write<TResult>(Func<DbContext, Task<TResult>> fn)
        {
            using (var context = this.factory.Create())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var result = await fn(context);
                        await context.SaveChangesAsync();
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        public async Task<TResult> WriteOptimistically<TResult>(Func<DbContext, Task<TResult>> fn)
        {
            using (var context = this.factory.Create())
            {
                var saved = false;
                var saveAttempts = 0;
                DbUpdateConcurrencyException lastConcurrentException = null;
                while (!saved && (saveAttempts++ < this.maxSaveAttempts || this.maxSaveAttempts == -1))
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = await fn(context);
                            await context.SaveChangesAsync();
                            transaction.Commit();
                            return result;
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            await transaction.RollbackAsync();
                            logger.LogDebug("Concurrency Failure", ex);
                            lastConcurrentException = ex;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                throw lastConcurrentException;
            }
        }

    }
}
