using Veza.OAA;
using Veza.OAA.Application;
using Veza.OAA.Exceptions;

namespace Veza.OAATest.ApplicationTest
{
    [TestClass]
    public class FullApplicationTest
    {
        [TestMethod]
        public void TestSimpleCustomApplication()
        {
            CustomApplication customApp = new(name: "testapp", applicationType: "csharp", description: "this is a test");
            
            // add resources
            List<Dictionary<string, string?>> resources = new()
            {
                new Dictionary<string, string?>
                {
                    {"name", "resource1" },
                    {"resource_type", "rtype1" },
                    {"description", "First resource" }
                },
                new Dictionary<string, string?>
                {
                    {"name", "resource2" },
                    {"resource_type", "rtype1" },
                    {"description", "Second resource" }
                },
                new Dictionary<string, string?>
                {
                    {"name", "resource3" },
                    {"resource_type", "rtype1" },
                    {"description", "Third resource" }
                },
                new Dictionary<string, string?>
                {
                    {"name", "resource4" },
                    {"resource_type", "rtype1" },
                    {"description", null }
                }
            };
            foreach(Dictionary<string, string?> item in resources)
            {
                _ = customApp.AddResource(name: item["name"], resourceType: item["resource_type"], description: item["description"]);
            }
            Assert.AreEqual(4, customApp.Resources.Count);

            // add custom app tag
            customApp.AddTag(name: "AppTag", value: "test");
            Assert.AreEqual(1, customApp.Tags.Count);

            // create custom permissions
            customApp.AddCustomPermission(name: "read", permissions: new List<Permission>() { Permission.DataRead, Permission.MetadataRead });
            customApp.AddCustomPermission(name: "write", permissions: new List<Permission>() { Permission.DataWrite, Permission.MetadataWrite });
            customApp.AddCustomPermission(name: "meta", permissions: new List<Permission>() { Permission.NonData, Permission.MetadataRead });
            Assert.AreEqual(3, customApp.CustomPermissions.Count);

            // add users
            customApp.AddUser(name: "user1");
            customApp.Users["user1"].AddTag(name: "mytag");
            customApp.Users["user1"].AddTag(name: "mytag"); //ensure that adding a duplicate tag is a no-op
            customApp.AddUser(name: "user2", identities: new List<string>() { "user2@example.com" });
            Assert.AreEqual(2, customApp.Users.Count);
            Assert.AreEqual(1, customApp.Users["user1"].Tags.Count);
            Assert.AreEqual(1, customApp.Users["user2"].Identities.Count);

            // add groups
            customApp.AddGroup("group1");
            customApp.Groups["group1"].AddTag("tag1");
            customApp.AddGroup("group2");
            customApp.AddUser(name: "user3", groups: new List<string> { "group1" });
            customApp.AddUser(name: "user4", groups: new List<string> { "group1", "group2" });
            Assert.AreEqual(4, customApp.Users.Count);
            Assert.AreEqual(2, customApp.Groups.Count);
            Assert.AreEqual(1, customApp.Groups["group1"].Tags.Count);
            Assert.AreEqual(1, customApp.Users["user3"].Groups.Count);
            Assert.AreEqual(2, customApp.Users["user4"].Groups.Count);

            // add idp user
            customApp.AddIdPIdentity("okta_user1");
            Assert.AreEqual(1, customApp.IdPIdentities.Count);

            // add roles
            customApp.AddRole(name: "admin");
            customApp.Roles["admin"].AddPermissions(new List<string> { "read", "write", "meta" });
            customApp.AddRole(name: "viewer", permissions: new List<string> { "read" });
            Assert.AreEqual(2, customApp.Roles.Count);
            Assert.AreEqual(3, customApp.Roles["admin"].Permissions.Count);

            // add permissions to resources
            customApp.Users["user1"].AddPermission(permission: "read", new List<Resource> { customApp.Resources["resource1"] });
            customApp.Groups["group1"].AddPermission(permission: "write", new List<Resource> { customApp.Resources["resource2"] });
            customApp.IdPIdentities["okta_user1"].AddPermission(permission: "meta", new List<Resource> { customApp.Resources["resource3"] });
            Assert.AreEqual(1, customApp.Users["user1"].ResourcePermissions.Count);
            Assert.AreEqual(1, customApp.Groups["group1"].ResourcePermissions.Count);
        }

        [TestMethod]
        public void TestGenerateApp()
        {
            string savedJson = Base.ReadEmbeddedResource(
                "OAATest.OAA.Application.custom_application_payload.json");
            CustomApplication customApp = GenerateCustomApp.GenerateApp();
            string generatedJson = customApp.GetJSONPayload();
            Assert.AreEqual(savedJson, generatedJson);
        }

        [TestMethod]
        public void TestCustomProperties()
        {
            CustomApplication customApp = new(applicationType: "dotnet", name: "testapp", description: "This is a test");
            customApp.AddCustomPermission(name: "Admin", Permission.DataWrite);

            // application properties
            customApp.DefinedProperties[typeof(CustomApplication)].
                DefineProperty("contact", typeof(string));
            customApp.DefinedProperties[typeof(CustomApplication)].
                DefineProperty("version", typeof(string));
            customApp.Properties["contact"] = "billy";
            customApp.SetProperty(name: "version", value: "2022.1.1");
            Assert.ThrowsException<TemplateException>(() => customApp.SetProperty("not_set", "something"));

            // user properties
            customApp.DefinedProperties[typeof(User)].
                DefineProperty("guest", typeof(bool));
            User bob = customApp.AddUser("bob");
            bob.SetProperty("guest", true);
            var ex = Assert.ThrowsException<TemplateException>(() => bob.SetProperty("not_set", "something"));
            Assert.IsTrue(ex.Message.Contains("not_set"));
            User sue = customApp.AddUser("sue");

            // group properties
            customApp.DefinedProperties[typeof(Group)].
                DefineProperty("built_in", typeof(bool));
            customApp.DefinedProperties[typeof(Group)].
                DefineProperty("group_email", typeof(string));
            Group admins = customApp.AddGroup("admins");
            Group operators = customApp.AddGroup("operators");
            admins.SetProperty("group_email", "admins@example.com");
            operators.SetProperty("built_in", true);

            // resource properties
            customApp.DefineResourceProperty("owner", typeof(string), "thing");
            customApp.DefineResourceProperty("private", typeof(bool), "thing");
            Resource thing1 = customApp.AddResource("thing1", "thing", "test description");
            thing1.SetProperty("owner", "jeff");
            Resource sub_thing = thing1.AddSubResource("sub_thing", "thing");
            sub_thing.SetProperty("owner", "fred");
            Resource cog1 = customApp.AddResource("cog1", "cog");

            // exception when no properties are set on a resource
            ex = Assert.ThrowsException<TemplateException>(() => cog1.SetProperty("not_set", "something"));

            // exception when the property being set doesn't exist
            ex = Assert.ThrowsException<TemplateException>(() => thing1.SetProperty("not_set", "something"));

            // exception with invalid property name
            Assert.ThrowsException<TemplateException>(() =>
                customApp.DefinedProperties[typeof(User)].
                    DefineProperty("in-valid!!!1", typeof(string)));
            Assert.ThrowsException<TemplateException>(() =>
                customApp.DefinedProperties[typeof(Group)].
                    DefineProperty("in-valid!!!1", typeof(string)));
            Assert.ThrowsException<TemplateException>(() =>
                customApp.DefinedProperties[typeof(Role)].
                    DefineProperty("in-valid!!!1", typeof(string)));

            // get app payload and validate properties are present
            Dictionary<string, object> payload = customApp.GetPayload();
            CollectionAssert.Contains(payload.Keys, "custom_property_definition");

        }
    }
}
