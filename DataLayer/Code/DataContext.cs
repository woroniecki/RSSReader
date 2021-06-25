﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DataLayer.Models;
using DataLayer.Code.Configs;

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
            //optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new IdentityUserConfig());
            modelBuilder.ApplyConfiguration(new IdentityRoleConfig());
            modelBuilder.ApplyConfiguration(new IdentityUserLoginConfig());
            modelBuilder.ApplyConfiguration(new IdentityUserRoleConfig());
            modelBuilder.ApplyConfiguration(new IdentityUserTokenConfig());
            modelBuilder.ApplyConfiguration(new IdentityUserClaimConfig());
            modelBuilder.ApplyConfiguration(new IdentityRoleClaimConfig());
            modelBuilder.ApplyConfiguration(new BlogConfig());
        }
    }
}
