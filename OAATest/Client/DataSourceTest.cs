using RestSharp;
using Veza.OAA.Client;
using static Veza.Sdk.Client.ApiClient;

namespace Veza.OAATest.ClientTest
{
    [TestClass]
    public class DataSourceTest
    {
        private readonly string _api_key;
        private readonly string _provider_icon;
        private readonly string _url;
        public DataSourceTest()
        {
            Base.SetEnvironmentVariablesFromSecrets();
            _api_key = Environment.GetEnvironmentVariable("api_key") ?? "";
            _provider_icon = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48c3ZnIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgd2lkdGg9IjEiIGhlaWdodD0iMSIvPg==";
            _url = Environment.GetEnvironmentVariable("url") ?? "";
        }


        [TestMethod]
        public async Task TestCreateDataSourceWithOptons()
        {
            Guid test_uuid = Guid.NewGuid();
            string provider_name = $"MsTest-{test_uuid}";
            string data_source_name = $"DataSource-{test_uuid}";
            OAAClient oaa_client = new(_api_key, _url);

            RestResponse<VezaApiResponse> create_provider_response = await oaa_client.CreateProvider(provider_name, "application");
            string provider_id = create_provider_response.Data.value["id"].ToString();

            Dictionary<string, string> options = new() { { "test", "test" } };
            RestResponse<VezaApiResponse> create_data_source_response = await oaa_client.CreateDataSource(data_source_name: data_source_name, provider_id: provider_id, options: options);
            Assert.IsTrue(create_data_source_response.IsSuccessful);
            string data_source_id = create_data_source_response.Data.value["id"].ToString();

            RestResponse delete_data_source_response = await oaa_client.DeleteDataSource(data_source_id: data_source_id, provider_id: provider_id);
            Assert.IsTrue(delete_data_source_response.IsSuccessful);

            RestResponse delete_provider_response = await oaa_client.DeleteProvider(provider_id: provider_id);
            Assert.IsTrue(delete_provider_response.IsSuccessful);
        }

        [TestMethod]
        public async Task TestDataSourceLifecycle()
        {
            // create a provider
            Guid test_uuid = Guid.NewGuid();
            string provider_name = $"MsTest-{test_uuid}";
            string data_source_name = $"DataSource-{test_uuid}";
            OAAClient oaa_client = new(_api_key, _url);

            RestResponse<VezaApiResponse> create_provider_response = await oaa_client.CreateProvider(provider_name, "application");
            string provider_id = create_provider_response.Data.value["id"].ToString();

            // create a data source
            RestResponse<VezaApiResponse> create_data_source_response = await oaa_client.CreateDataSource(data_source_name: data_source_name, provider_id: provider_id);
            Assert.IsTrue(create_data_source_response.IsSuccessful);
            string data_source_id = create_data_source_response.Data.value["id"].ToString();

            // get data source by name
            Dictionary<string, object> get_data_source_response = await oaa_client.GetDataSource(data_source_name: data_source_name, provider_id: provider_id);
            Assert.IsNotNull(get_data_source_response);

            // get data source by id
            Dictionary<string, object> get_ds_by_id_response = await oaa_client.GetDataSource(data_source_id: data_source_id, provider_id: provider_id);
            Assert.IsNotNull(get_ds_by_id_response);

            // delete data source
            RestResponse delete_data_source_response = await oaa_client.DeleteDataSource(data_source_id: data_source_id, provider_id: provider_id);
            Assert.IsTrue(delete_data_source_response.IsSuccessful);

            // delete provider
            RestResponse delete_provider_response = await oaa_client.DeleteProvider(provider_id: provider_id);
            Assert.IsTrue(delete_provider_response.IsSuccessful);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "GetDataSource called without a data_source_id or data_source_name; one is required")]
        public async Task TestGetDataSourceWithoutIdOrName()
        {
            OAAClient oaa_client = new(_api_key, _url);
            // Dictionary<string, object> response = await oaa_client.GetDataSource(provider_id: "test");
            await oaa_client.GetDataSource(provider_id: "test");
        }
    }
}
