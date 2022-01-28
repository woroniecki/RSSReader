using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class ApiUser : IdentityUser
    {
        [JsonIgnore]
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

        public async Task<string> GetRoleAndAssignToUserIfNull(UserManager<ApiUser> userManager)
        {
            var user_role = await userManager.GetRolesAsync(this);
            if (user_role.Count <= 0)
            {
                await userManager.AddToRoleAsync(this, "User");
                user_role = await userManager.GetRolesAsync(this);
            }

            return user_role.First();
        }

        public async Task SetRole(string role, UserManager<ApiUser> userManager)
        {
            var roles = await userManager.GetRolesAsync(this);
            await userManager.RemoveFromRolesAsync(this, roles.ToArray());
            await userManager.AddToRoleAsync(this, role);
        }
    }
}
