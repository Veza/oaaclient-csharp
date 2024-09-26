using Microsoft.Extensions.Configuration;
using RestSharp;
using Veza.Sdk.Client;


namespace Veza.VezaTest
{
    [TestClass]
    public class ClientTest
    {
        private string _api_key;
        private string _url;

        public ClientTest()
        {
            SetEnvironmentVariablesFromSecrets();
            _api_key = Environment.GetEnvironmentVariable("api_key");
            _url = Environment.GetEnvironmentVariable("url");
        }

        [TestMethod]
        public void TestConstructor() 
        {
            ApiClient api_client = new(api_key: _api_key, url: _url);
            Assert.IsNotNull(api_client.configuration);
            Assert.AreEqual($"https://{_url}/", api_client.configuration.URL);
        }

        [TestMethod]
        public void TestConstructorWithFullUrl() 
        {
            string full_url = $"https://{_url}/";
            ApiClient api_client = new(api_key: _api_key, url: full_url);
            Assert.IsNotNull(api_client.configuration);
            Assert.AreEqual(full_url, api_client.configuration.URL);
        }

        [TestMethod]
        public async Task TestApiGet()
        {
            ApiClient api_client = new(api_key: _api_key, url: _url);
            RestResponse<ApiClient.VezaApiResponse> response = await api_client.ApiGet("/api/v1/providers/custom/templates");
            Assert.IsTrue(response.Data.values.First().ContainsKey("name"));
            Assert.IsTrue(response.IsSuccessful);
        }

        static void SetEnvironmentVariablesFromSecrets()
        {
            var configuration = new ConfigurationBuilder().AddUserSecrets<ClientTest>().Build();
            foreach (var child in configuration.GetChildren())
            {
                Environment.SetEnvironmentVariable(child.Key, child.Value);
            }
        }
    }
}
