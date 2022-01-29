using DataLayer.Code;
using DataLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicLayer.PopulateDB
{
    public static class ApiAuthorization
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<DataContext>();

            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            string[] roleNames = { "Admin", "User", "Test" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            context.SaveChanges();
        }
        
        public static async Task CreateAdmin(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<DataContext>();
            var user_manager = serviceProvider.GetRequiredService<UserManager<ApiUser>>();
            IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();

            //Here you could create a super user who will maintain the web app
            var poweruser = new ApiUser
            {

                UserName = config.GetSection("Admin:Username").Get<string>(),
                Email = config.GetSection("Admin:Email").Get<string>()
            };

            //Ensure you have these values in your appsettings.json file
            string userPWD = config.GetSection("Admin:Password").Get<string>();
            var user = await user_manager.FindByEmailAsync(config.GetSection("Admin:Email").Get<string>());

            if (user == null)
            {
                var createPowerUser = await user_manager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await user_manager.AddToRoleAsync(poweruser, "Admin");

                }
            }

            context.SaveChanges();
        }
    }
}
