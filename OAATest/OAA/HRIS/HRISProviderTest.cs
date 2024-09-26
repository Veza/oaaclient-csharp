using Veza.OAA.HRIS;

namespace Veza.OAATest.HRISTest
{
    [TestClass]
    public class HRISProviderTest
    {

        [TestMethod]
        public void TestGenerateHRISProvider()
        {
            string saved_json = Base.ReadEmbeddedResource("OAATest.OAA.HRIS.hris_provider_payload.json");
            HRISProvider provider = GenerateHRIS.GenerateHRISProvider();
            string generated_json = provider.GetJSONPayload();
            Assert.AreEqual(saved_json, generated_json);
        }
    }
}