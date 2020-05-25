using System;
using Microsoft.EntityFrameworkCore;

namespace Awesome.EntityFramework
{
    public static class AwesomeEntityFramework
    {
        public static IAwesomeDbContextFactory<TContext> DefaultFactory<TContext>(
            DbContextOptions<TContext> options,
            Func<DbContextOptions<TContext>, TContext> createFn) where TContext : DbContext {
            return new DefaultAwesomeDbContextFactory<TContext>(options, createFn);
        }
    }
}