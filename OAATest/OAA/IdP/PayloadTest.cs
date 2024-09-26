using Veza.OAA.Client;
using Veza.OAA.IdP;
using RestSharp;
using static Veza.Sdk.Client.ApiClient;

namespace Veza.OAATest.IdPTest
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
            string name = $"MsTest IdPTest - {testUUID}";

            OAAClient oaaClient = new(_api_key, _url);
            IdPProvider provider = GenerateIdP.GenerateIdPProvider();
            await oaaClient.CreateProvider(provider_name: name, custom_template: "identity_provider");
            await oaaClient.PushIdP(provider_name: name, data_source_name: name, provider);

            // Comment above awaits and uncomment these to debug responses
            //var provider_create_response = await oaaClient.CreateProvider(provider_name: name, custom_template: "idp");
            //var push_response = await oaaClient.PushIdP(provider_name: name, data_source_name: name, provider);

            /// get provider by name
            VezaApiResponse get_by_name_response = await oaaClient.GetProvider(provider_name: name);
            string provider_id = get_by_name_response.values.First()["id"].ToString();

            /// delete provider
            RestResponse delete_response = await oaaClient.DeleteProvider(provider_id: provider_id);
            Assert.IsTrue(delete_response.IsSuccessful);
        }
    }
}