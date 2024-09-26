using Veza.OAA;
using Veza.OAA.Exceptions;
using Veza.OAA.Filesystem;

namespace Veza.OAATest.FilesystemTest
{
    [TestClass]
    public class FilesystemTest
    {
        [TestMethod]
        public void TestFilesystem()
        {
            List<OAA.Permission> permissions_rw = new()
            { 
                OAA.Permission.DataRead, 
                OAA.Permission.DataWrite
            };
            List<OAA.Permission> permissions_read = new()
            { 
                OAA.Permission.DataRead, 
                OAA.Permission.MetadataRead
            };
            List<OAA.Permission> permissions_full = new()
            { 
                OAA.Permission.DataRead, 
                OAA.Permission.DataWrite,
                OAA.Permission.MetadataRead, 
                OAA.Permission.MetadataWrite,
                OAA.Permission.DataCreate, 
                OAA.Permission.DataDelete
            };

            Filesystem filesystem = new();

            filesystem.AddServer("test server");
            filesystem.AddMount("test mount 1", uniqueId: "mount1");
            filesystem.Mounts["mount1"].AddTag("tag1", "tag1_value");
            filesystem.AddMount("test mount 2", uniqueId: "mount2");
            filesystem.AddFolder("test folder 1", parentId: "mount1", path: "/mount1/folder1");
            filesystem.AddFolder("test folder 2", parentId: "mount2", path: "/mount2/folder2");
            filesystem.AddPermission("rw", permissions_rw);
            filesystem.AddPermission("read", permissions_read);
            filesystem.AddPermission("full", permissions_full);
        }

        [TestMethod]
        public void TestCustomProperties()
        {
            // setup
            Filesystem filesystem = new();

            #region "Property Definitions"
            // filesystem
            filesystem.DefinedProperties[typeof(Filesystem)].
                DefineProperty("samba_version", typeof(string));
            filesystem.DefinedProperties[typeof(Filesystem)].
                DefineProperty("pii", typeof(bool));
            filesystem.DefinedProperties[typeof(Filesystem)].
                DefineProperty("filesystem_property", typeof(bool));

            // folder
            filesystem.DefinedProperties[typeof(Folder)].
                DefineProperty("owner", typeof(string));
            filesystem.DefinedProperties[typeof(Folder)].
                DefineProperty("hidden", typeof(bool));

            // mount
            filesystem.DefinedProperties[typeof(Mount)].
                DefineProperty("department_share", typeof(bool));
            filesystem.DefinedProperties[typeof(Mount)].
                DefineProperty("dfs", typeof(bool));

            // permission
            filesystem.DefinedProperties[typeof(OAA.Filesystem.Permission)].
                DefineProperty("contains_advanced_attributes", typeof(bool));
            filesystem.DefinedProperties[typeof(OAA.Filesystem.Permission)].
                DefineProperty("allows_xattrs", typeof(bool));

            // server
            filesystem.DefinedProperties[typeof(Server)].
                DefineProperty("operating_system", typeof(string));
            filesystem.DefinedProperties[typeof(Server)].
                DefineProperty("another_property", typeof(int));

            // name overlap
            filesystem.DefinedProperties[typeof(Filesystem)].
                DefineProperty("test_prop", typeof(bool));
            filesystem.DefinedProperties[typeof(Server)].
                DefineProperty("test_prop", typeof(bool));

            #endregion

            #region "Filesystem Properties"
            filesystem.Properties["samba_version"] = "4.18";
            filesystem.SetProperty("pii", true);

            Assert.AreEqual(2, filesystem.Properties.Count);
            Assert.AreEqual("4.18", filesystem.Properties["samba_version"]);
            Assert.AreEqual(true, filesystem.Properties["pii"]);
            Assert.ThrowsException<TemplateException>(() => filesystem.SetProperty("not_set", "something"));
            #endregion

            #region "Folder Properties"
            Folder folder = filesystem.AddFolder("test folder", "abc123", "/path/to/folder");
            folder.SetProperty("owner", "testuser@example.com");
            folder.SetProperty("hidden", true);
            
            Assert.AreEqual(2, folder.Properties.Count, 2);
            Assert.AreEqual("testuser@example.com", folder.Properties["owner"]);
            Assert.AreEqual(true, folder.Properties["hidden"]);
            Assert.ThrowsException<TemplateException>(() => folder.SetProperty("not_set", "something"));
            Assert.ThrowsException<TemplateException>(() => folder.SetProperty("filesystem_property", true));
            #endregion

            #region "Mount Properties"
            Mount mount = filesystem.AddMount("test mount");
            mount.SetProperty("department_share", true);
            mount.SetProperty("dfs", true);

            Assert.AreEqual(2, mount.Properties.Count);
            Assert.AreEqual(true, mount.Properties["department_share"]);
            Assert.AreEqual(true, mount.Properties["dfs"]);
            Assert.ThrowsException<TemplateException>(() => mount.SetProperty("not_set", "something"));
            Assert.ThrowsException<TemplateException>(() => mount.SetProperty("filesystem_property", true));
            #endregion

            #region "Permission Properties"
            List<OAA.Permission> oaa_permissions = new()
                { OAA.Permission.DataRead, OAA.Permission.DataWrite};
            OAA.Filesystem.Permission permission = filesystem.AddPermission("test permission", oaa_permissions);
            permission.SetProperty("contains_advanced_attributes", true);
            permission.SetProperty("allows_xattrs", true);

            Assert.AreEqual(2, permission.Properties.Count);
            Assert.AreEqual(true, permission.Properties["contains_advanced_attributes"]);
            Assert.AreEqual(true, permission.Properties["allows_xattrs"]);
            Assert.ThrowsException<TemplateException>(() => permission.SetProperty("not_set", "something"));
            Assert.ThrowsException<TemplateException>(() => permission.SetProperty("filesystem_property", true));
            #endregion

            #region "Server Properties"
            Server server = filesystem.AddServer("test server");
            server.SetProperty("operating_system", "Windows Server 2022");
            server.SetProperty("another_property", 10);

            Assert.AreEqual(2, server.Properties.Count);
            Assert.AreEqual("Windows Server 2022", server.Properties["operating_system"]);
            Assert.AreEqual(10, server.Properties["another_property"]);
            Assert.ThrowsException<TemplateException>(() => server.SetProperty("not_set", "something"));
            Assert.ThrowsException<TemplateException>(() => server.SetProperty("filesystem_property", true));
            #endregion
        }
    }
}
