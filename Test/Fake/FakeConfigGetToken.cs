using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Fake
{
    public class FakeConfigGetToken : Mock<IConfiguration>
    {
        public static string KEY = "you cant break me, secret key";
        public FakeConfigGetToken()
        {
            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(x => x.Value).Returns(KEY);
            Setup(x => x.GetSection("AppSettings:Token"))
                .Returns(configSectionMock.Object)
                .Verifiable();
        }
    }
}
