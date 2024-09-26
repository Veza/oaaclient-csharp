using Veza.OAA.Application;
using Veza.OAA.Client;
using RestSharp;
using static Veza.Sdk.Client.ApiClient;

namespace Veza.OAATest.ApplicationTest
{
    [TestClass]
    public class PayloadTest
    {
        private readonly string _api_key;
        private readonly string _url;

        public PayloadTest()
        {
            Base.SetEnvironmentVariablesFromSecrets();
            _api_key = Environment.GetEnvironmentVariable("api_key") ?? "";
            _url = Environment.GetEnvironmentVariable("url") ?? "";
        }

        [TestMethod]
        public async Task TestPayloadPush()
        {
            Guid testUUID = Guid.NewGuid();
            string name = $"MsTest Application - {testUUID}";

            OAAClient oaaClient = new(_api_key, _url);
            CustomApplication app = GenerateCustomApp.GenerateApp();
            await oaaClient.CreateProvider(provider_name: name, custom_template: "application");
            await oaaClient.PushApplication(provider_name: name, data_source_name: name, app);

            // Comment above awaits and uncomment these to debug responses
            //var provider_create_response = oaaClient.CreateProvider(provider_name: name, custom_template: "application");
            //RestResponse<ApiClient.VezaApiResponse> push_response = oaaClient.PushApplication(provider_name: name, data_source_name: name, app);

            /// get provider by name
            VezaApiResponse get_by_name_response = await oaaClient.GetProvider(provider_name: name);
            string provider_id = get_by_name_response.values.First()["id"].ToString();

            /// delete provider
            RestResponse delete_response = await oaaClient.DeleteProvider(provider_id: provider_id);
            Assert.IsTrue(delete_response.IsSuccessful);
        }
    }
}
