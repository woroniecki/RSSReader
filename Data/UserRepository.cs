using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RSSReader.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApiUser> _userManager;

        public UserRepository(UserManager<ApiUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<ApiUser> GetCurrentUser(Controller controller)
        {
            var id = controller.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return await _userManager.FindByIdAsync(id);
        }
    }

    public interface IUserRepository
    {
        Task<ApiUser> GetCurrentUser(Controller controller);
    }
}
