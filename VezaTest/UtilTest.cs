using Veza.Sdk;

namespace Veza.VezaTest
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void TestParseURLs()
        {
            string parsed_url = Util.ParseURL("mstest.example.com");
            string expected_https = "https://mstest.example.com/";
            string expected_http = "http://mstest.example.com/";

            Assert.AreEqual(parsed_url, expected_https);

            parsed_url = Util.ParseURL("mstest.example.com/");
            Assert.AreEqual(parsed_url, expected_https);

            parsed_url = Util.ParseURL("https://mstest.example.com");
            Assert.AreEqual(parsed_url, expected_https);

            parsed_url = Util.ParseURL("https://mstest.example.com/");
            Assert.AreEqual(parsed_url, expected_https);

            parsed_url = Util.ParseURL("http://mstest.example.com");
            Assert.AreEqual(parsed_url, expected_http);

            parsed_url = Util.ParseURL("http://mstest.example.com/");
            Assert.AreEqual(parsed_url, expected_http);
        }

        [TestMethod]
        public void TestHttpMethodToRestSharp()
        {
            RestSharp.Method method = HttpMethod.Get.ToRestSharp();
            Assert.AreEqual(RestSharp.Method.Get, method);
            method = HttpMethod.Post.ToRestSharp();
            Assert.AreEqual(RestSharp.Method.Post, method);
        }

    }
}