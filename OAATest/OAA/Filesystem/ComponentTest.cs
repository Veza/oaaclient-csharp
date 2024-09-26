using Veza.OAA.Exceptions;
using Veza.OAA.Filesystem;

namespace Veza.OAATest.FilesystemTest
{
    [TestClass]
    public class ComponentTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            Filesystem filesystem = new();
            Assert.IsNotNull(filesystem);
            Assert.AreEqual(filesystem.FilesystemType, "Windows File Server");
        }

        [TestMethod]
        public void TestAddFolderObject()
        {
            Filesystem filesystem = new();
            Assert.AreEqual(filesystem.Folders.Count, 0);
            Folder folder = new(name: "test folder 1",
                parentId: "abc123", path: "/share1/folder1", uniqueId: "def987");
            filesystem.AddFolder(folder);
            Assert.AreEqual(filesystem.Folders.Count, 1);

            // ensure duplicate UniqueIds can't be created
            Folder folder2 = new(name: "test folder 2",
                parentId: "abc123", path: "/share1/folder2", uniqueId: "def987");
            Assert.ThrowsException<TemplateException>(() => filesystem.AddFolder(folder2));
            Assert.AreEqual(filesystem.Folders.Count, 1);
        }

        [TestMethod]
        public void TestAddFolder()
        {
            Filesystem filesystem = new();
            Assert.AreEqual(filesystem.Folders.Count, 0);
            filesystem.AddFolder(name: "test folder", parentId: "abc123",
                path: "/share1/folder1");
            Assert.AreEqual(filesystem.Folders.Count, 1);
        }

        [TestMethod]
        public void TestAddMountObject()
        {
            Filesystem filesystem = new();
            Assert.AreEqual(filesystem.Mounts.Count, 0);
            Mount mount = new(name: "test mount",
                description: "this is a test mount", uniqueId: "def987");
            filesystem.AddMount(mount);
            Assert.AreEqual(filesystem.Mounts.Count, 1);

            // ensure duplicate UniqueIds can't be created
            Mount mount2 = new(name: "test mount 2",
                description: "this is a test mount 2", uniqueId: "def987");
            Assert.ThrowsException<TemplateException>(() => filesystem.AddMount(mount2));
            Assert.AreEqual(filesystem.Mounts.Count, 1);
        }

        [TestMethod]
        public void TestAddMount()
        {
            Filesystem filesystem = new();
            Assert.AreEqual(filesystem.Mounts.Count, 0);
            filesystem.AddMount(name: "test mount");
            Assert.AreEqual(filesystem.Mounts.Count, 1);
        }

        [TestMethod]
        public void TestAddPermissionObject()
        {
            List<OAA.Permission> oaaPermissions = new()
                { OAA.Permission.DataRead, OAA.Permission.DataWrite};
           Permission permission = new(name: "test permission",
                permissions: oaaPermissions);
            Filesystem filesystem = new();
            Assert.AreEqual(filesystem.Permissions.Count, 41);
            filesystem.AddPermission(permission);
            Assert.AreEqual(filesystem.Permissions.Count, 42);
        }

        [TestMethod]
        public void TestAddPermission()
        {
            List<OAA.Permission> oaaPermissions = new()
                { OAA.Permission.DataRead, OAA.Permission.DataWrite};
            Filesystem filesystem = new();
            Assert.AreEqual(filesystem.Permissions.Count, 41);
            filesystem.AddPermission(name: "test permission", permissions: oaaPermissions);
            Assert.AreEqual(filesystem.Permissions.Count, 42);
        }

        [TestMethod]
        public void TestAddServerObject()
        {
            Server server = new(name: "test server", uniqueId: "def987");
            Filesystem filesystem = new();
            Assert.IsNull(filesystem.Server);
            filesystem.AddServer(server);
            Assert.IsNotNull(filesystem.Server);

            // ensure only one server can exist
            Server server2 = new(name: "test server 2", uniqueId: "def987");
            Assert.ThrowsException<TemplateException>(() => filesystem.AddServer(server2));
            Assert.IsNotNull(filesystem.Server);
        }

        [TestMethod]
        public void TestAddServer()
        {
            Filesystem filesystem = new();
            Assert.IsNull(filesystem.Server);
            filesystem.AddServer(name: "test server");
            Assert.IsNotNull(filesystem.Server);
        }
    }

    [TestClass]
    public class FolderTest
    {
        [TestMethod] 
        public void TestConstructor() 
        {
            Folder folder = new(name: "test folder",
                parentId: "abc123", path: "/share1/folder1");

            Assert.IsNotNull(folder);
            Assert.IsNull(folder.CreatedAt);
            Assert.IsNull(folder.Description);
            Assert.IsFalse(folder.InheritParentPermissions);
            Assert.AreEqual(folder.Name, "test folder");
            Assert.AreEqual(folder.UniqueId, "test folder");
        }

        [TestMethod]
        public void TestConstructorFull()
        {
            DateTime dtCreatedAt = DateTime.ParseExact("01/01/2000", "d", System.Globalization.CultureInfo.InvariantCulture);
            DateTime dtUpdatedAt = DateTime.ParseExact("12/31/2010", "d", System.Globalization.CultureInfo.InvariantCulture);
            Folder folder = new(name: "test folder",
                parentId: "abc123", path: "/share1/folder1",
                createdAt: dtCreatedAt, description: "this is a test folder",
                inheritParentPermissions: true, uniqueId: "def987",
                updatedAt: dtUpdatedAt);

            Assert.IsNotNull(folder);
            Assert.IsNotNull(folder.CreatedAt);
            Assert.AreEqual(folder.Description, "this is a test folder");
            Assert.AreEqual(folder.Name, "test folder");
            Assert.AreEqual(folder.UniqueId, "def987");
            Assert.IsNotNull(folder.UpdatedAt);
        }

        [TestMethod]
        public void TestPermission()
        {
            List<OAA.Permission> permissions_read = new()
            {
                OAA.Permission.DataRead,
                OAA.Permission.MetadataRead
            };
            Permission _ = new(
                name: "read", 
                permissions: permissions_read, 
                description: "read permission"
            );
            Folder folder = new(name: "test folder",
                parentId: "abc123", path: "/share1/folder1");
            folder.AddPermissions("user_1@example.com", "read");
            Assert.AreEqual(1, folder.IdentityToPermissions.Count);
            Assert.IsNotNull(folder.IdentityToPermissions["user_1@example.com"]);
            Assert.AreEqual(1, folder.IdentityToPermissions["user_1@example.com"].Count);
        }

        [TestMethod]
        public void TestPermissions()
        {
            List<OAA.Permission> permissions_read = new()
            {
                OAA.Permission.DataRead,
                OAA.Permission.MetadataRead
            };
            List<OAA.Permission> permissions_write = new()
            {
                OAA.Permission.DataWrite,
                OAA.Permission.MetadataWrite
            };
            Permission _ = new(
                name: "read",
                permissions: permissions_read,
                description: "read permission"
            );
            _ = new(
                name: "write",
                permissions: permissions_write,
                description: "write permission"
            );
            Folder folder = new(name: "test folder",
                parentId: "abc123", path: "/share1/folder1");
            folder.AddPermissions("user_1@example.com", new List<string>() { "read", "write" });
            Assert.AreEqual(1, folder.IdentityToPermissions.Count);
            Assert.IsNotNull(folder.IdentityToPermissions["user_1@example.com"]);
            Assert.AreEqual(2, folder.IdentityToPermissions["user_1@example.com"].Count);
        }
    }

    [TestClass]
    public class MountTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            Mount mount = new(name: "test mount");

            Assert.IsNotNull(mount);
            Assert.IsNull(mount.CreatedAt);
            Assert.IsNull(mount.Description);
            Assert.AreEqual(mount.Name, "test mount");
            Assert.IsNull(mount.UpdatedAt);
            Assert.AreEqual(mount.UniqueId, "test mount");
        }

        [TestMethod]
        public void TestConstructorFull()
        {
            DateTime dtCreatedAt = DateTime.ParseExact("01/01/2000", "d", System.Globalization.CultureInfo.InvariantCulture);
            DateTime dtUpdatedAt = DateTime.ParseExact("12/31/2010", "d", System.Globalization.CultureInfo.InvariantCulture);
            Mount mount = new(name: "test mount",
                createdAt: dtCreatedAt, description: "this is a test mount",
                uniqueId: "def987", updatedAt: dtUpdatedAt);

            Assert.IsNotNull(mount);
            Assert.IsNotNull(mount.CreatedAt);
            Assert.AreEqual(mount.Description, "this is a test mount");
            Assert.AreEqual(mount.Name, "test mount");
            Assert.AreEqual(mount.UniqueId, "def987");
            Assert.IsNotNull(mount.UpdatedAt);
        }

        [TestMethod]
        public void TestPermission()
        {
            List<OAA.Permission> permissions_read = new()
            {
                OAA.Permission.DataRead,
                OAA.Permission.MetadataRead
            };
            Permission _ = new(
                name: "read",
                permissions: permissions_read,
                description: "read permission"
            );
            Mount mount = new(name: "test mount", uniqueId: "test_mount_1");
            mount.AddPermissions("user_1@example.com", "read");
            Assert.AreEqual(1, mount.IdentityToPermissions.Count);
            Assert.IsNotNull(mount.IdentityToPermissions["user_1@example.com"]);
            Assert.AreEqual(1, mount.IdentityToPermissions["user_1@example.com"].Count);
        }

        [TestMethod]
        public void TestPermissions()
        {
            List<OAA.Permission> permissions_read = new()
            {
                OAA.Permission.DataRead,
                OAA.Permission.MetadataRead
            };
            List<OAA.Permission> permissions_write = new()
            {
                OAA.Permission.DataWrite,
                OAA.Permission.MetadataWrite
            };
            Permission _ = new(
                name: "read",
                permissions: permissions_read,
                description: "read permission"
            );
            _ = new(
                name: "write",
                permissions: permissions_write,
                description: "write permission"
            );
            Mount mount = new(name: "test mount", uniqueId: "test_mount_1");
            mount.AddPermissions("user_1@example.com", new List<string>() { "read", "write" });
            Assert.AreEqual(1, mount.IdentityToPermissions.Count);
            Assert.IsNotNull(mount.IdentityToPermissions["user_1@example.com"]);
            Assert.AreEqual(2, mount.IdentityToPermissions["user_1@example.com"].Count);
        }
    }

    [TestClass]
    public class PermissionTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            List<OAA.Permission> oaaPermissions = new() 
                { OAA.Permission.DataRead, OAA.Permission.DataWrite};
            Permission permission = new(name: "test permission",
                permissions: oaaPermissions);
            Assert.IsNotNull(permission);
            Assert.AreEqual(permission.Name, "test permission");
            Assert.IsNull(permission.Description);
            Assert.AreEqual(permission.PermissionType.Count, 2);
            Assert.IsNull(permission.UniqueId);
        }

        [TestMethod]
        public void TestConstructorFull()
        {
            List<OAA.Permission> oaaPermissions = new()
                { OAA.Permission.DataRead, OAA.Permission.DataWrite};
            Permission permission = new(name: "test permission",
                permissions: oaaPermissions, description: "this is a test permission");
            Assert.IsNotNull(permission);
            Assert.AreEqual(permission.Description, "this is a test permission");
            Assert.AreEqual(permission.Name, "test permission");
            Assert.IsNull(permission.UniqueId);
            Assert.AreEqual(permission.PermissionType.Count, 2);
        }
    }

    [TestClass]
    public class ServerTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            Server server = new(name: "test server");
            
            Assert.IsNotNull(server);
            Assert.IsNull(server.Description);
            Assert.AreEqual(server.Name, "test server");
            Assert.AreEqual(server.UniqueId, "test server");
        }

        [TestMethod]
        public void TestConstructorFull()
        {
            Server server = new(name: "test server", 
                description: "this is a test server",
                uniqueId: "def987");

            Assert.IsNotNull(server);
            Assert.AreEqual(server.Description, "this is a test server");
            Assert.AreEqual(server.Name, "test server");
            Assert.AreEqual(server.UniqueId, "def987");
        }
    }
}