using Microsoft.EntityFrameworkCore;

namespace Awesome.EntityFramework
{
    public interface IAwesomeDbContextFactory<TContext> where TContext : DbContext
    {
        TContext Create();
    }
}
