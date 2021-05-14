using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using DataLayer.Code;

namespace Tests.Helpers
{
    public static class InMemoryDb
    {
        public static DbContextOptions<DataContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an in-memory database
            var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString()) //the database name is set to a unique Guid
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}
