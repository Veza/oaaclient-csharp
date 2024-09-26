using Veza.Sdk.Client;
using Veza.Sdk.Exceptions;

namespace Veza.VezaTest
{
    [TestClass]
    public class ComponentTest
    {
        [TestMethod]
        public void TestConfiguration() 
        {
            Configuration config = new (api_key: "testkey", url: "mstest.example.com");
            Assert.IsTrue(config.EnableCompression);
            Assert.IsTrue(config.VerifySSL);
            Assert.AreEqual("https://mstest.example.com/", config.URL);
            Assert.AreEqual("vezaclient/1.0 csharp/12.0 windows", config.UserAgent);
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException), "API key validation failed")]
        public void TestMissingApiKey()
        {
            Configuration _ = new(api_key: "", url: "mstest.example.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException), "URL validation failed")]
        public void TestMissingUrl()
        {
            Configuration _ = new(api_key: "testkey", url: "");
        }

        [TestMethod]
        public void TestDisableVerifySSL()
        {
            Configuration config = new(api_key: "testkey", url: "mstest.example.com");
            config.VerifySSL = false;
            Assert.IsFalse(config.VerifySSL);
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException), "URL validation failed")]
        public void TestHTTPwithVerifySSLEnabled()
        {
            Configuration config = new(api_key: "testkey", url: "http://mstest.example.com");
            Configuration.Validate(config);
        }

        [TestMethod]
        public void TestHTTPwithVerifySSLDisabled()
        {
            Configuration config = new(api_key: "testkey", url: "http://mstest.example.com");
            config.VerifySSL = false;
            Configuration.Validate(config);
            Assert.IsFalse(config.VerifySSL);
        }
            
    }
}
