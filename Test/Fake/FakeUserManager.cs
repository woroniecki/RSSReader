using Microsoft.AspNetCore.Identity;
using Moq;

using DataLayer.Models;

namespace Tests.Fake
{
    public class FakeUserManager : Mock<UserManager<ApiUser>>
    {
        public FakeUserManager()  : base
            (Mock.Of<IUserStore<ApiUser>>(), null, null, null, null, null, null, null, null)
        {
        }
    }
}
