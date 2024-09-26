using System.Reflection;
using Veza.OAA;
using Veza.OAA.Filesystem;

namespace Veza.OAATest.FilesystemTest
{
    internal static class GenerateFilesystem
    {
        internal static Filesystem GenerateFS()
        {
            // filesystem object
            Filesystem fs = new();

            #region "Permissions"
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
            fs.AddPermission(
                name: "custom_rw",
                permissions: permissions_rw,
                description: "read-write"
            );
            fs.AddPermission(
                name: "custom_read",
                permissions: permissions_read,
                description: "custom_read"
            );
            fs.AddPermission(
                name: "custom_full",
                permissions: permissions_full,
                description: "full control"
            );
            #endregion

            #region "Properties"
            // filesystem
            fs.DefinedProperties[typeof(Filesystem)].
                DefineProperty("samba_version", typeof(string));
            fs.DefinedProperties[typeof(Filesystem)].
                DefineProperty("pii", typeof(bool));

            // folder
            fs.DefinedProperties[typeof(Folder)].
                DefineProperty("owner", typeof(string));
            fs.DefinedProperties[typeof(Folder)].
                DefineProperty("hidden", typeof(bool));

            // mount
            fs.DefinedProperties[typeof(Mount)].
                DefineProperty("department_share", typeof(bool));
            fs.DefinedProperties[typeof(Mount)].
                DefineProperty("dfs", typeof(bool));

            // permission
            fs.DefinedProperties[typeof(OAA.Filesystem.Permission)].
                DefineProperty("contains_advanced_attributes", typeof(bool));
            fs.DefinedProperties[typeof(OAA.Filesystem.Permission)].
                DefineProperty("allows_xattrs", typeof(bool));

            // server
            fs.DefinedProperties[typeof(Server)].
                DefineProperty("operating_system", typeof(string));
            fs.DefinedProperties[typeof(Server)].
                DefineProperty("another_property", typeof(int));
            #endregion

            #region "Filesystem"
            fs.SetProperty("samba_version", "4.18");
            fs.SetProperty("pii", true);
            #endregion

            #region "Server"
            fs.AddServer("test server", "this is a test server", "test_server_1");
            fs.Server.SetProperty("operating_system", "Windows Server 2022");
            fs.Server.SetProperty("another_property", 10);
            #endregion

            #region "Mounts"
            fs.AddMount("test mount 1", "2001-01-01T00:00:00.000Z".FromRFC3339(), "this is test mount 1", "test_mount_1");
            fs.AddMount("test mount 2", "2002-02-02T00:00:00.000Z".FromRFC3339(), "this is test mount 2", "test_mount_2", "2020-07-01T00:00:00.000Z".FromRFC3339());
            fs.Mounts["test_mount_1"].SetProperty("department_share", true);
            fs.Mounts["test_mount_2"].SetProperty("dfs", false);
            fs.Mounts["test_mount_1"].AddPermissions("test_user_1@example.com", "custom_full");
            fs.Mounts["test_mount_2"].AddPermissions("test_user_2@example.com", new List<string>() { "custom_read", "custom_rw" });

            #endregion

            #region "Folders"
            Folder folder = fs.AddFolder(
                name: "folder_1",
                parentId: "test_mount_1",
                path: "/test_mount_1/folder_1",
                createdAt: "2021-01-01T00:00:00.000Z".FromRFC3339(),
                description: "this is test folder 1",
                inheritParentPermissions: false,
                uniqueId: "test_folder_1",
                updatedAt: "2021-02-01T00:00:00.000Z".FromRFC3339()
            );
            folder.SetProperty("owner", "admin@example.com");
            folder.SetProperty("hidden", false);
            folder.AddPermissions("test_user_2@example.com", "custom_read");

            folder = fs.AddFolder(
                name: "folder_1a",
                parentId: "test_folder_1",
                path: "/test_mount_1/folder_1/folder_1a",
                createdAt: "2021-01-02T00:00:00.000Z".FromRFC3339(),
                description: "this is test folder 1a",
                inheritParentPermissions: false,
                uniqueId: "test_folder_1a",
                updatedAt: "2021-02-02T00:00:00.000Z".FromRFC3339()
            );
            folder.SetProperty("hidden", true);
            folder.AddPermissions("test_user_3@example.com", "custom_rw");

            folder = fs.AddFolder(
                name: "folder_2",
                parentId: "test_mount_2",
                path: "/test_mount_2/folder_2",
                createdAt: "2022-01-01T00:00:00.000Z".FromRFC3339(),
                description: "this is test folder 2",
                inheritParentPermissions: false,
                uniqueId: "test_folder_2",
                updatedAt: "2022-02-01T00:00:00.000Z".FromRFC3339()
            );
            folder.SetProperty("owner", "storage_admin@example.com");
            folder.AddPermissions("test_user_1@example.com", "custom_read");

            fs.AddFolder(
                name: "folder_2a",
                parentId: "test_folder_2",
                path: "/test_mount_2/folder_2/folder_2a",
                createdAt: "2022-01-02T00:00:00.000Z".FromRFC3339(),
                description: "this is test folder 2a",
                inheritParentPermissions: true,
                uniqueId: "test_folder_2a",
                updatedAt: "2022-02-02T00:00:00.000Z".FromRFC3339()
            );
            #endregion

            #region "Tags"
            fs.Mounts["test_mount_1"].AddTag("mount_tag_1", "mount_tag_1_value");
            fs.Mounts["test_mount_2"].AddTag("mount_tag_2");
            fs.Server.AddTag("server_tag_1", "server_tag_1_value");
            fs.Server.AddTag("server_tag_2");
            fs.Folders["test_folder_1"].AddTag("folder_tag_1", "folder_tag_1_value");
            fs.Folders["test_folder_1a"].AddTag("folder_tag_1a");
            fs.Folders["test_folder_2"].AddTag("folder_tag_2");
            #endregion

            return fs;
        }
    }
}
