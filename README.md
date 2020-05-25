# Awesome Entity Framework Core

This repo is an example of an awesome way to use EntityFramework Core.

## Just show me how it works

Once it's set up, you get access to an IAwesomeContext which has 3 methods:

* Read - Read from the database
* Write - Write to the database
* WriteOptimistically - Write Optimistically, i.e. retry if a concurrency exception is thrown

e.g.:

```c#

// assuming a model like this
class MyModel 
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Points { get; set; }
    [Timestamp]
    public byte[] Timestamp { get; set; }
}

// read from the database
var model = await context.Read(async db => await db.FindAsync<MyModel>(id));

// write to the database
var bob = await context.Write(async db => 
{
    var result = await db.AddAsync(new MyModel {
        Id = Guid.NewGuid(),
        Name = "Bob"
    });
    return result.Entity;
});

// write optimistically
var bob = await context.Write(async db => 
{
    var bob = await db.FindAsync<MyModel>(id);
    // add 1 to whatever number of points bob currently has
    bob.Points += 1;
    return bob;
});

```

## Setting it up

```c#
// create a DbContext
public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) {}
    public DbSet<MyModel> MyModels { get; set; }
}

// create an AwesomeDbContextFactory
public class MyAwesomeDbContextFactory : IAwesomeDbContextFactory<MyDbContext>
{
    private readonly DbContextOptions<MyDbContext> options;

    public MyAwesomeDbContextFactory(DbContextOptions<MyDbContext> options)
    {
        this.options = options;
    }
    public MyDbContext Create()
    {
        return new TestDbContext(options);
    }
}
```

## Testing it

See this [Awesome Test](https://github.com/jamiemccrindle/awesome-entity-framework/blob/master/src/Awesome.EntityFramework.Tests/AwesomeTest.cs)