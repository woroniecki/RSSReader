using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DataLayer.Models;
using DataLayer.Code.Configs;
using System;

namespace DataLayer.Code
{
    public class DataContext : IdentityDbContext<ApiUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<UserPostData> UserPostDatas { get; set; }
        public DbSet<Group> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.LogTo(Console.WriteLine);
            //optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new BlogConfig());
        }
    }
}
