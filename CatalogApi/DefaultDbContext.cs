using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CatalogApi
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public class DefaultDbContext : DbContext
    {
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }

    //for migrations
    public class DefaultDbContextDesignFactory : IDesignTimeDbContextFactory<DefaultDbContext>
    {
        public DefaultDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration["APPSETTINGS:DBSETTINGS:CONNECTIONSTRING"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Db connection string is null or empty");

            var optionsBuilder = new DbContextOptionsBuilder<DefaultDbContext>()
                .UseNpgsql(connectionString);

            return new DefaultDbContext(optionsBuilder.Options);
        }
    }
}