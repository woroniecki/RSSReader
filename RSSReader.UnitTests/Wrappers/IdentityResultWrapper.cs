using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Identity;
using RSSReader.Controllers;
using Microsoft.Extensions.Configuration;

namespace RSSReader.UnitTests.Wrappers
{
    public class IdentityResultWrapper : IdentityResult
    {
        public IdentityResultWrapper(bool succeeded)
        {
            Succeeded = succeeded;
        }
    }
}
