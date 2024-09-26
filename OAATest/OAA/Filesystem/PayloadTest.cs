using RestSharp;
using Veza.OAA.Client;
using Veza.OAA.Filesystem;
using Veza.Sdk.Client;

namespace Veza.OAATest.FilesystemTest
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
        public void TestGenerateFilesystem()
        {
            string server_payload = Base.ReadEmbeddedResource(
                "OAATest.OAA.Filesystem.filesystem_server_payload.json");
            string folder_payload = Base.ReadEmbeddedResource(
                "OAATest.OAA.Filesystem.filesystem_folder_payload.json");
            Filesystem fs = GenerateFilesystem.GenerateFS();
            string generated_server_json = fs.ServerJSON();
            Assert.AreEqual(server_payload, generated_server_json);
            string generated_folder_json = fs.FolderJSON();
            Assert.AreEqual(folder_payload, generated_folder_json);
        }

        [TestMethod]
        public async Task TestProviderPush()
        {
            OAAClient oaa_client = new(_api_key, _url);
            RestResponse<ApiClient.VezaApiResponse> response = await oaa_client.CreateProvider("MSTest Filesystem", "file_system");
            Assert.IsTrue(response.IsSuccessful);

            // clean up the provider
            await oaa_client.DeleteProvider(provider_id: response.Data.value["id"].ToString());
        }

        [TestMethod]
        public async Task TestServerPush()
        {
            OAAClient oaa_client = new(_api_key, _url);
            Filesystem fs = GenerateFilesystem.GenerateFS();
            RestResponse<ApiClient.VezaApiResponse> provider_response = await oaa_client.CreateProvider("MSTest Filesystem", "file_system");
            Assert.IsTrue(provider_response.IsSuccessful);
            RestResponse<ApiClient.VezaApiResponse> server_response = await oaa_client.PushServer("MSTest Filesystem", "fs1", fs);
            Assert.IsTrue(server_response.IsSuccessful);
            
            // clean up the provider
            await oaa_client.DeleteProvider(provider_id: provider_response.Data.value["id"].ToString());
        }

        [TestMethod]
        public async Task TestServerandFolderPush()
        {
            OAAClient oaa_client = new(_api_key, _url);
            Filesystem fs = GenerateFilesystem.GenerateFS();
            RestResponse<ApiClient.VezaApiResponse> provider_response = await oaa_client.CreateProvider("MSTest Filesystem", "file_system");
            Assert.IsTrue(provider_response.IsSuccessful);
            RestResponse<ApiClient.VezaApiResponse> server_response = await oaa_client.PushServer("MSTest Filesystem", "fs1_server", fs);
            Assert.IsTrue(server_response.IsSuccessful);
            RestResponse<ApiClient.VezaApiResponse> folder_response = await oaa_client.PushFolder("MSTest Filesystem", "fs1_folder", fs);
            Assert.IsTrue(folder_response.IsSuccessful);

            // clean up the provider
            await oaa_client.DeleteProvider(provider_id: provider_response.Data.value["id"].ToString());
        }
    }
}
