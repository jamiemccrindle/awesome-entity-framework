using System;
using Microsoft.EntityFrameworkCore;

namespace Awesome.EntityFramework
{
    public class DefaultAwesomeDbContextFactory<TContext> : IAwesomeDbContextFactory<TContext> where TContext : DbContext
    {
        private readonly DbContextOptions<TContext> options;
        private readonly Func<DbContextOptions<TContext>, TContext> createFn;

        public DefaultAwesomeDbContextFactory(
            DbContextOptions<TContext> options,
            Func<DbContextOptions<TContext>, TContext> createFn)
        {
            this.options = options;
            this.createFn = createFn;
        }
        public TContext Create()
        {
            return createFn(options);
        }
    }
}
