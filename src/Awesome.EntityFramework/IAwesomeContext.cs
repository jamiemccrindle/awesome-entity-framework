using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Awesome.EntityFramework
{
    public interface IAwesomeContext
    {
        Task<TResult> Read<TResult>(Func<DbContext, Task<TResult>> fn);
        Task<TResult> Write<TResult>(Func<DbContext, Task<TResult>> fn);
        Task<TResult> WriteOptimistically<TResult>(Func<DbContext, Task<TResult>> fn, int maxSaveAttempts);
    }
}
