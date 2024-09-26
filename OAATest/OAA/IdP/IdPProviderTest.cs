using Veza.OAA.IdP;

namespace Veza.OAATest.IdPTest
{
    [TestClass]
    public class IdPProviderTest
    {
        [TestMethod]
        public void TestGenerateIdPProvider()
        {
            string saved_json = Base.ReadEmbeddedResource("OAATest.OAA.IdP.idp_provider_payload.json");
            IdPProvider provider = GenerateIdP.GenerateIdPProvider();
            string generated_json = provider.GetJSONPayload();
            Assert.AreEqual(saved_json, generated_json);
        }
    }
}