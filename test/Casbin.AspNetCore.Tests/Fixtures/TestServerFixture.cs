using Casbin.AspNetCore.Authorization;
using Casbin.AspNetCore.Tests.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Casbin;

namespace Casbin.AspNetCore.Tests.Fixtures
{
    public class TestServerFixture
    {
        public TestServerFixture()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.ConfigureServices(services =>
            {
                services.AddCasbinAuthorization(options =>
                {
                    options.DefaultModelPath = TestUtility.GetExampleFile("basic_model.conf");
                    options.DefaultPolicyPath = TestUtility.GetExampleFile("basic_policy.csv");
                })
                .BuildServiceProvider();
            });
            webHostBuilder.Configure(app =>
            {
                app.UseCasbinAuthorization();
            });
            TestServer = new TestServer(webHostBuilder);
        }

        public TestServer TestServer { get; }
    }
}
