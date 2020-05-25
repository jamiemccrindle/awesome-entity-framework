using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Awesome.EntityFramework.Tests
{

    public class AwesomeModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [Timestamp]
        public byte[] Timestamp { get; set; }

    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public DbSet<AwesomeModel> AwesomeModels { get; set; }
    }

    public class TestAwesomeDbContextFactory : IAwesomeDbContextFactory<TestDbContext>
    {
        private readonly DbContextOptions<TestDbContext> options;

        public TestAwesomeDbContextFactory(DbContextOptions<TestDbContext> options)
        {
            this.options = options;
        }
        public TestDbContext Create()
        {
            return new TestDbContext(options);
        }
    }

    public class AwesomeTest
    {
        [Fact]
        public async Task TestReadAndWrite()
        {
            var factory = new TestAwesomeDbContextFactory(
                new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

            var logger = new LoggerFactory().CreateLogger<AwesomeContext<TestDbContext>>();
            var context = new AwesomeContext<TestDbContext>(logger, factory);
            var id = Guid.NewGuid();
            var added = await context.Write(async db => {
                var result = await db.AddAsync(new AwesomeModel {
                    Id = id,
                    Name = "Bob"
                } );
                return result.Entity;
            });
            Assert.Equal("Bob", added.Name);
            var read = await context.Read(async db => await db.FindAsync<AwesomeModel>(id));
            Assert.Equal("Bob", read.Name);
        }
    }
}
