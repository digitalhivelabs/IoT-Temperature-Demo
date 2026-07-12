using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace AcmeLogisticsApi.Tests.Integration
{
    public class IntegrationTestBase: IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly WebApplicationFactory<Program> Factory;
        protected readonly HttpClient _client;
        protected readonly IConfiguration Configuration;

        public IntegrationTestBase(WebApplicationFactory<Program> factory)
        {
            Factory = factory;
            _client = factory.CreateClient();
            Configuration = factory.Services.GetRequiredService<IConfiguration>();
        }
    }
}
