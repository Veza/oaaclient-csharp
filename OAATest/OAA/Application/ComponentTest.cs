using Veza.OAA;
using Veza.OAA.Application;
using Veza.OAA.Base;
using Veza.OAA.Exceptions;
using System.Globalization;

namespace Veza.OAATest.ApplicationTest
{
    [TestClass]
    public class EnumTest
    {
        [TestMethod]
        public void TestIdentityTypeToString()
        {
            IdentityType localUser = IdentityType.local_user;
            Assert.IsTrue(localUser.ToString() == "local_user");
        }
        [TestMethod]
        public void TestIdPProviderTypeToString()
        {
            IdPProviderType activeDirectory = IdPProviderType.active_directory;
            Assert.IsTrue(activeDirectory.ToString() == "active_directory");
        }

        [TestMethod]
        public void TestPermissionToString()
        {
            Permission permission = Permission.DataWrite;
            Assert.IsTrue(permission.ToString() == "DataWrite");
        }
    }

    [TestClass]
    public class CustomApplicationTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            CustomApplication app = new ("test app", "test app 1", "this is a test app");
            Assert.IsNotNull(app);
            Assert.IsTrue(app.Name == "test app 1");
            Assert.IsTrue(app.ToString() == "Custom Application test app 1 - test app");
        }

        [TestMethod]
        public void AddCustomPermissions()
        {
            CustomApplication app = new ("test app", "test app 1", "this is a test app");
            app.AddCustomPermission("read", new List<Permission> { Permission.DataRead, Permission.MetadataRead }, false, null);
            Assert.AreEqual(1, app.CustomPermissions.Count);
        }

        [TestMethod]
        public void AddCustomPermission()
        {
            CustomApplication app = new ("test app", "test app 1", "this is a test app");
            app.AddCustomPermission("read", Permission.DataRead, false, null);
            Assert.AreEqual(1, app.CustomPermissions.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(TemplateException),"Custom permission read already exists")]
        public void AddDuplicateCustomPermission()
        {
            CustomApplication app = new ("test app", "test app 1", "this is a test app");
            app.AddCustomPermission("read", new List<Permission> { Permission.DataRead, Permission.MetadataRead }, false, null);
            app.AddCustomPermission("read", new List<Permission> { Permission.DataRead }, false, null);
        }
    }

    [TestClass]
    public class CustomPermissionTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            CustomPermission customPermission = new ("read", 
                new List<Permission> { Permission.DataRead, Permission.MetadataRead }, 
                applyToSubResources: false, 
                resourceTypes: null);

            
            Assert.IsTrue(customPermission.Name == "read");
            CollectionAssert.AreEqual(customPermission.Permissions, new List<Permission> { Permission.DataRead, Permission.MetadataRead });

        }

        [TestMethod]
        public void TestAddResources()
        {
            CustomPermission customPermission = new ("read",
                new List<Permission> { Permission.DataRead, Permission.MetadataRead },
                applyToSubResources: false,
                resourceTypes: null);

            customPermission.AddResourceType("test_type");
            CollectionAssert.AreEqual(customPermission.ResourceTypes, new List<string> { "test_type" });
        }
    }

    [TestClass]
    public class LocalGroupTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            Group testGroup = new (
                name: "test_group"
            );

            Assert.AreEqual("test_group", testGroup.Name);
        }

        [TestMethod]
        public void TestAddGroup()
        {
            Group testGroup = new (
                name: "test_group"
            );
            Assert.AreEqual(0, testGroup.Groups.Count);
            testGroup.AddGroup("test_subgroup");
            Assert.AreEqual(1, testGroup.Groups.Count);
            Assert.AreEqual("test_subgroup", testGroup.Groups.First());
        }

        [TestMethod]
        public void TestAddGroupToItself()
        {
            Group testGroup = new (
                name: "test_group"
            );
            Assert.ThrowsException<TemplateException>(() => 
                testGroup.AddGroup("test_group"));
        }
    }

    [TestClass]
    public class LocalRoleTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            Role testRole = new (name: "test_role");
            Assert.AreEqual("test_role", testRole.Name);
            Assert.AreEqual("Role test_role (test_role)", testRole.ToString());

            Role testRoleWithId = new (name: "test_role_with_id", uniqueId: "test_role_id");
            Assert.AreEqual("test_role_id", testRoleWithId.UniqueId);
            Assert.AreEqual("Role test_role_with_id (test_role_id)", testRoleWithId.ToString());
        }

        [TestMethod]
        public void TestAddPermission()
        {
            Role testRole = new (name: "test_role");
            Assert.AreEqual(0, testRole.Permissions.Count);
            testRole.AddPermission("read");
            Assert.AreEqual(1, testRole.Permissions.Count);
            CollectionAssert.Contains(testRole.Permissions, "read");
        }

        [TestMethod]
        public void TestAddPermissions()
        {
            Role testRole = new (name: "test_role");
            Assert.AreEqual(0, testRole.Permissions.Count);
            testRole.AddPermissions(new List<string> { "read", "write" });
            Assert.AreEqual(2, testRole.Permissions.Count);
            CollectionAssert.Contains(testRole.Permissions, "read");
            CollectionAssert.Contains(testRole.Permissions, "write");
        }
    }

    [TestClass]
    public class LocalUserTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            User user = new (
                name: "Test User",
                identities: new List<string> { "test_user@example.com" },
                uniqueId: "test_user@example.com"
            );

            Assert.AreEqual("Test User", user.Name);
            Assert.AreEqual("Local user - Test User (test_user@example.com)", user.ToString());
        }

        [TestMethod]
        public void TestAddGroup()
        {
            User user = new (
                name: "Test User",
                identities: new List<string> { "test_user@example.com" },
                uniqueId: "test_user@example.com"
            );

            user.AddGroup("test_group");
            CollectionAssert.Contains(user.Groups, "test_group");
        }

        [TestMethod]
        public void TestAddIdentity()
        {
            User user = new (
                name: "Test User",
                uniqueId: "test_user@example.com"
            );

            Assert.AreEqual(0, user.Identities.Count);
            user.AddIdentity("test_user@example.com");
            Assert.AreEqual(1, user.Identities.Count);
        }

        [TestMethod]
        public void TestAddIdentities()
        {
            User user = new (
                name: "Test User",
                uniqueId: "test_user@example.com"
            );

            Assert.AreEqual(0, user.Identities.Count);
            user.AddIdentities(new List<string> { "test_user@example.com", "test@example.com" });
            Assert.AreEqual(2, user.Identities.Count);
        }
    }

    [TestClass]
    public class ResourceTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            Resource resource = new (
                name: "test_resource",
                description: "this is a test resource",
                applicationName: "testapp",
                resourceType: "test_type",
                propertyDefinitions: new PropertyDefinitions()
            );

            Assert.AreEqual("test_resource", resource.Key);

            resource = new (
                name: "test_resource",
                description: "this is a test resource",
                applicationName: "testapp",
                key: "test_key",
                resourceType: "test_type",
                propertyDefinitions: new PropertyDefinitions()
            );

            Assert.AreEqual("test_key", resource.Key);

            resource = new (
                name: "test_resource",
                description: "this is a test resource",
                applicationName: "testapp",
                resourceType: "test_type",
                propertyDefinitions: new PropertyDefinitions(),
                uniqueId: "test_unique_id"
            );

            Assert.AreEqual("test_unique_id", resource.Key);
            Assert.AreEqual("Resource: test_resource (test_unique_id) - test_type", resource.ToString());
        }

        [TestMethod]
        public void AddSubResource()
        {
            Resource resource = new (
                name: "test_resource",
                description: "this is a test resource",
                applicationName: "testapp",
                resourceType: "test_type",
                propertyDefinitions: new PropertyDefinitions()
            );

            resource.AddSubResource(
                name: "test_subresource",
                description: "this is a test subresource",
                resourceType: "test_type",
                uniqueId: "test_unique_id");
        
            Assert.AreEqual(1, resource.SubResources.Count);
            Assert.AreEqual("test_resource.test_unique_id", resource.SubResources["test_unique_id"].Key);
        }

        [TestMethod]
        [ExpectedException(typeof(TemplateException), "Cannot add subresource...")]
        public void TestAddDuplicateSubresource()
        {
            Resource resource = new (
                name: "test_resource",
                description: "this is a test resource",
                applicationName: "testapp",
                resourceType: "test_type",
                propertyDefinitions: new PropertyDefinitions()
            );

            resource.AddSubResource(
                name: "test_subresource",
                description: "this is a test subresource",
                resourceType: "test_type",
                uniqueId: "test_unique_id");

            resource.AddSubResource(
                name: "other_test_subresource",
                description: "this is a second est subresource",
                resourceType: "test_type",
                uniqueId: "test_unique_id");
        }

        [TestMethod]
        public void TestAddTag()
        {
            Resource resource = new (
                name: "test_resource",
                description: "this is a test resource",
                applicationName: "testapp",
                resourceType: "test_type",
                propertyDefinitions: new PropertyDefinitions()
            );
            Assert.AreEqual(0, resource.Tags.Count);
            resource.AddTag("tag_key", "tag_value");
            Assert.AreEqual(1, resource.Tags.Count);
        }
    }

    [TestClass]
    public class TagTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            Tag testTag = new ("test_key");
            Assert.AreEqual("test_key", testTag.Key);

            testTag = new Tag("test_key2", "test_value");
            Assert.AreEqual("test_key2", testTag.Key);
            Assert.AreEqual("test_value", testTag.Value);
            Assert.AreEqual("Tag test_key2: test_value", testTag.ToString() );
        }

        [TestMethod]
        public void TestToJson()
        {
            Tag testTag = new("test_key", "test_value");
            Assert.AreEqual("{\"Key\":\"test_key\",\"Value\":\"test_value\"}", testTag.ToJson());
        }

        [TestMethod]
        [ExpectedException(typeof(TemplateException), "Invalid characters in tag key...")]
        public void TestConstructorWithInvalidKey()
        {
            _ = new Tag("$%&*#(($*abc");
        }

        [TestMethod]
        [ExpectedException(typeof(TemplateException), "Invalid characters in tag value...")]
        public void TestConstructorWithInvalidValue()
        {
            _ = new Tag("valid_key", "$% &*#(($*abc");
        }
    }

    [TestClass]
    public class UtilTest
    {
        /// TODO: put this somewhere else
        [TestMethod]
        public void TestRFC3339Conversion()
        {
            DateTime dt1 = "2001-01-01T00:00:00.000Z".FromRFC3339();
            DateTime dt2 = "2002-02-01T00:00:00.000Z".FromRFC3339();
            string dt3 = dt2.ToRFC3339();

            Assert.AreEqual(DateTime.ParseExact("2001-01-01T00:00:00.000Z", "yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal), dt1);
            Assert.AreEqual(dt3, "2002-02-01T00:00:00.000Z");


        }
    }
}