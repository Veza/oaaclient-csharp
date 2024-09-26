using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Veza.OAA.Client;
using static Veza.Sdk.Client.ApiClient;

namespace Veza.OAATest.ClientTest
{
    [TestClass]
    public class ProviderTest
    {
        private readonly string _api_key;
        private readonly string _provider_icon;
        private readonly string _url;
        public ProviderTest()
        {
            Base.SetEnvironmentVariablesFromSecrets();
            _api_key = Environment.GetEnvironmentVariable("api_key") ?? "";
            _url = Environment.GetEnvironmentVariable("url") ?? "";
            _provider_icon = "PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHdpZHRoPSc2NicgaGVpZ2h0PSc2Nicgdmlld0JveD0nMCAwIDY2IDY2Jz48cGF0aC8+PC9zdmc+Cg==";
        }

        [TestMethod]
        public async Task TestCreateProviderWithOptions()
        {
            Guid test_uuid = Guid.NewGuid();
            string provider_name = $"MsTest-{test_uuid}";
            OAAClient oaa_client = new(_api_key, _url);
            Dictionary<string, string> options = new() { { "test", "test" } };

            /// create new provider
            RestResponse<VezaApiResponse> create_response = await oaa_client.CreateProvider(provider_name: provider_name, custom_template: "application", options: options);
            Assert.IsTrue(create_response.IsSuccessful);
            string provider_id = create_response.Data.value["id"].ToString();

            /// delete provider
            RestResponse delete_response = await oaa_client.DeleteProvider(provider_id: provider_id);
            Assert.IsTrue(delete_response.IsSuccessful);
        }

        [TestMethod]
        public async Task TestGetProviders()
        {
            OAAClient oaa_client = new(_api_key, _url);
            RestResponse response = await oaa_client.GetProviders();
            Assert.IsNotNull(response);

            // Uncomment below to debug response
            //var response_data = JsonConvert.DeserializeObject<JObject>(response.Content);
            //var response_values = response_data["values"];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "GetProvider called without a provider_id or provider_name; one is required")]
        public async Task TestGetProviderWithoutIdOrName()
        {
            OAAClient oaa_client = new(_api_key, _url);
            VezaApiResponse response = await oaa_client.GetProvider();
        }

        [TestMethod]
        public async Task TestProviderLifecycle()
        {
            Guid test_uuid = Guid.NewGuid();
            string provider_name = $"MsTest-{test_uuid}";
            OAAClient oaa_client = new(_api_key, _url);

            /// create new provider
            RestResponse<VezaApiResponse> create_response = await oaa_client.CreateProvider(provider_name: provider_name, custom_template: "application");
            Assert.IsTrue(create_response.IsSuccessful);

            string provider_id = create_response.Data.value["id"].ToString();

            /// get provider by id
            VezaApiResponse get_response = await oaa_client.GetProvider(provider_id: provider_id);
            Assert.IsNotNull(get_response);

            /// get provider by name
            VezaApiResponse get_by_name_response = await oaa_client.GetProvider(provider_name: provider_name);
            Assert.IsNotNull(get_by_name_response);

            /// delete provider
            RestResponse delete_response = await oaa_client.DeleteProvider(provider_id: provider_id);
            Assert.IsTrue(delete_response.IsSuccessful);
        }

        [TestMethod]
        public async Task TestUpdateProviderIcon()
        {
            Guid test_uuid = Guid.NewGuid();
            string provider_name = $"MsTest-{test_uuid}";
            OAAClient oaa_client = new(_api_key, _url);

            /// create new provider
            RestResponse<VezaApiResponse> create_response = await oaa_client.CreateProvider(provider_name, "application");
            Assert.IsTrue(create_response.IsSuccessful);
            string provider_id = create_response.Data.value["id"].ToString();

            /// update provider icon
            RestResponse update_response = await oaa_client.UpdateProviderIcon(provider_id, _provider_icon);
            Assert.IsTrue(update_response.IsSuccessful);

            /// delete provider
            RestResponse delete_response = await oaa_client.DeleteProvider(provider_id: provider_id);
            Assert.IsTrue(delete_response.IsSuccessful);
        }
    }
}
