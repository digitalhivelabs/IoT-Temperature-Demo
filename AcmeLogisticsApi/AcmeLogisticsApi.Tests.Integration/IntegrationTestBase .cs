using Microsoft.AspNetCore.Mvc.Testing;

namespace AcmeLogisticsApi.Tests.Integration
{
    public class IntegrationTestBase: IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly HttpClient _client;

        public IntegrationTestBase(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }
    }
}
