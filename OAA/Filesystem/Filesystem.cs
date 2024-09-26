using System.Diagnostics;
using System.Text.Json;
using Veza.OAA.Exceptions;
using Veza.OAA.Base;

namespace Veza.OAA.Filesystem
{

    /// <summary>
    /// Class for modeling filesystem authorization using the OAA Filesystem template
    /// 
    /// The Filesystem class consists of servers, mounts, folders, permissions,
    /// and permissions assignments and produces the OAA JSON payload for the filesystem template.
    /// 
    /// </summary>
    public class Filesystem
    {
        public Dictionary<Type, PropertyDefinitions> DefinedProperties { get; set; }
        public string FilesystemType { get; set; }
        public Dictionary<string, Folder> Folders { get; set; }
        public Dictionary<string, Mount> Mounts { get; set; }
        public Dictionary<string, Permission> Permissions { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public Server? Server { get; set; }

        public Filesystem(string filesystem_type = "Windows File Server")
        {
            DefinedProperties = new Dictionary<Type, PropertyDefinitions>()
            {
                { typeof(Filesystem), new PropertyDefinitions() },
                { typeof(Folder), new PropertyDefinitions() },
                { typeof(Mount), new PropertyDefinitions() },
                { typeof(Permission), new PropertyDefinitions() },
                { typeof(Server), new PropertyDefinitions() },
            };
            FilesystemType = filesystem_type;
            Folders = [];
            Mounts = [];
            Permissions = [];
            Properties = [];
            Server = null;

            PopulateDefaultPermissions();
        }

        /// <summary>
        /// Add an existing Folder object to the Filesystem object
        /// </summary>
        /// <param name="folder">The existing Folder object</param>
        /// <returns>The provided Folder object</returns>
        public Folder AddFolder(Folder folder)
        {
            if (Folders.ContainsKey(folder.UniqueId))
            {
                throw new TemplateException($"Folder ID {folder.UniqueId} already exists");
            }

            Trace.TraceInformation($"Adding Folder {folder.Path}");
            folder.PropertyDefinitions ??= DefinedProperties[typeof(Folder)];
            Folders.Add(folder.UniqueId, folder);
            return folder;
        }

        /// <summary>
        /// Create a new Folder object and add it to the Filesystem object
        /// </summary>
        /// <param name="name">The string name of the folder</param>
        /// <param name="parentId">The string UniqueId of the parent folder/mount</param>
        /// <param name="path">The string filesystem path of the folder</param>
        /// <param name="createdAt">The DateTime at which the folder was created</param>
        /// <param name="description">The string description of the folder</param>
        /// <param name="inheritParentPermissions">A boolean indicating if the folder inherits permissions from its parent</param>
        /// <param name="uniqueId">The string unique ID of the folder</param>
        /// <param name="updatedAt">The DateTime at which the folder was last updated</param>
        /// <returns>The newly created Folder object</returns>
        public Folder AddFolder(string name, string parentId, string path,
            DateTime? createdAt = null, string? description = null,
            bool inheritParentPermissions = false, string? uniqueId = null,
            DateTime? updatedAt = null)
        {
            string identifier = uniqueId ?? name;
            if (Folders.ContainsKey(identifier))
            {
                throw new TemplateException($"Folder ID {identifier} already exists");
            }

            Folder folder = new(name: name, parentId: parentId, path: path, 
                createdAt: createdAt, description: description,
                inheritParentPermissions: inheritParentPermissions, 
                propertyDefinitions: DefinedProperties[typeof(Folder)], 
                uniqueId: uniqueId, updatedAt: updatedAt);
            return AddFolder(folder);
        }


        /// <summary>
        /// Add an existing Mount object to the Filesystem object
        /// </summary>
        /// <param name="mount">The existing Mount object</param>
        /// <returns>The provided Mount object</returns>
        public Mount AddMount(Mount mount)
        {
            if (Mounts.ContainsKey(mount.UniqueId))
            {
                throw new TemplateException($"Mount ID {mount.UniqueId} already exists");
            }

            Trace.TraceInformation($"Adding Mount {mount.Name}");
            mount.PropertyDefinitions ??= DefinedProperties[typeof(Mount)];
            Mounts.Add(mount.UniqueId, mount);
            return mount;
        }

        /// <summary>
        /// Create a new Mount object and add it to the Filesystem object
        /// </summary>
        /// <param name="name">The string name of the mount</param>
        /// <param name="createdAt">The DateTime at which the mount was created</param>
        /// <param name="description">The string description of the mount</param>
        /// <param name="uniqueId">The string unique ID of the mount</param>
        /// <param name="updatedAt">The DateTime at which the mount was last updated</param>
        /// <returns>The newly created Mount object</returns>
        public Mount AddMount(string name, DateTime? createdAt = null,
            string? description = null, string? uniqueId = null,
            DateTime? updatedAt = null)
        {
            string identifier = uniqueId ?? name;
            if (Mounts.ContainsKey(identifier))
            {
                throw new TemplateException($"Mount ID {identifier} already exists");
            }

            Mount mount = new(name: name, createdAt: createdAt, 
                description: description,
                propertyDefinitions: DefinedProperties[typeof(Mount)],
                uniqueId: uniqueId, updatedAt: updatedAt);
            return AddMount(mount);
        }

        /// <summary>
        /// Add an existing Permission object to the Filesystem object
        /// </summary>
        /// <param name="permission">The existing Permission object</param>
        /// <returns>The provided Permission object</returns>
        public Permission AddPermission(Permission permission)
        {
            if (Permissions.ContainsKey(permission.Name))
            {
                throw new TemplateException($"Permission ID {permission.Name} already exists");
            }

            Trace.TraceInformation($"Adding Permission {permission.Name}");
            permission.PropertyDefinitions ??= DefinedProperties[typeof(Permission)];
            Permissions.Add(permission.Name, permission);
            return permission;
        }

        /// <summary>
        /// Create a new Permission object and add it to the Filesystem object
        /// </summary>
        /// <param name="name">The string name of the permission</param>
        /// <param name="permissions">The list of the OAA.Template.Permissions (canonical permissions) that make up this permission set</param>
        /// <param name="description">The string description of the permission</param>
        /// <returns>The newly created Permission object</returns>
        public Permission AddPermission(string name, List<OAA.Permission> permissions,
            string? description = null)
        {
            string identifier = name;
            if (Permissions.ContainsKey(identifier))
            {
                throw new TemplateException($"Permission ID {identifier} already exists");
            }

            Permission permission = new(name: name, permissions: permissions, 
                description: description,
                propertyDefinitions: DefinedProperties[typeof(Permission)]
            );
            return AddPermission(permission);
        }

        /// <summary>
        /// Add an existing Server object to the Filesystem object
        /// </summary>
        /// <param name="server">The existing Server object</param>
        /// <returns>The provided Server object</returns>
        public Server AddServer(Server server)
        {
            if (Server is not null)
            {
                throw new TemplateException($"Filesystem already contains a Server object");
            }

            Trace.TraceInformation($"Adding Server {server.Name}");
            server.PropertyDefinitions ??= DefinedProperties[typeof(Server)];
            Server = server;
            return server;
        }

        /// <summary>
        /// Create a new Server object and add it to the Filesystem object
        /// </summary>
        /// <param name="name">The string name of the server</param>
        /// <param name="description">The string description of the server</param>
        /// <param name="uniqueId">The string unique id of the server</param>
        /// <returns>The newly created Server object</returns>
        public Server AddServer(string name, string? description = null,
            string? uniqueId = null)
        {
            if (Server is not null)
            {
                throw new TemplateException($"Filesystem already contains a Server object");
            }
            Server server = new(name: name, description: description, 
                propertyDefinitions: DefinedProperties[typeof(Server)], 
                uniqueId: uniqueId);
            return AddServer(server);
        }

        /// <summary>
        /// Generate the Veza payload for the Filesystem folder submission
        /// </summary>
        /// <returns>The Veza payload as a JSON string for upload</returns>
        public string FolderJSON()
        {
            return PayloadtoJSON(FolderPayload());
        }

        /// <summary>
        /// Generate a serializable Filesystem folder submission 
        /// </summary>
        /// <returns>The Veza payload as a dictionary</returns>
        public Dictionary<string, object> FolderPayload()
        {
            Trace.TraceInformation("Generating Filesystem Folder payload");
            Dictionary<string, object> custom_properties = new()
            {
                { "folder_properties", DefinedProperties[typeof(Folder)].ToDictionary() }
            };

            Dictionary<string, object> payload = new()
            {
                ["custom_property_definition"] = custom_properties,
                ["file_system_type"] = FilesystemType,
                ["folders"] = from folder in Folders.Values select folder.ToDictionary(),
                ["permissions"] = from permission in Permissions.Values select permission.ToDictionary()
            };

            if (Server is not null)
            {
                payload.Add("server_unique_id", Server.UniqueId);
            }

            return payload;
        }

        /// <summary>
        /// Serialize the Dictionary Veza payload to JSON for submission
        /// </summary>
        /// <param name="payload">The Dictionary payload to be serialized</param>
        /// <returns>The serialized JSON string</returns>
        public static string PayloadtoJSON(Dictionary<string, object> payload)
        {
            var json_options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(payload, json_options);
        }

        /// <summary>
        /// Add default Windows/SMB permissions to the Filesystem object
        /// </summary>
        private void PopulateDefaultPermissions()
        {
            AddPermission("AccessSystemSecurity", [OAA.Permission.MetadataRead]);
            AddPermission("AppendData", [Veza.OAA.Permission.DataWrite]);
            AddPermission("ChangePermissions", [OAA.Permission.MetadataWrite]);
            AddPermission("CreateDirectories", [OAA.Permission.DataWrite, OAA.Permission.MetadataCreate]);
            AddPermission("CreateFiles", [OAA.Permission.DataWrite]);
            AddPermission("Delete", [OAA.Permission.DataDelete]);
            AddPermission("DeleteSubdirectoriesAndFiles", [OAA.Permission.DataDelete, OAA.Permission.MetadataDelete]);
            AddPermission("ExecuteFile", [OAA.Permission.DataRead]);
            AddPermission("FileAppendData", [OAA.Permission.DataWrite]);
            AddPermission("FileDeleteChild", [OAA.Permission.DataDelete]);
            AddPermission("FileExecute", [OAA.Permission.NonData]);
            AddPermission("FileReadAttributes", [OAA.Permission.MetadataRead]);
            AddPermission("FileReadData", [OAA.Permission.DataRead]);
            AddPermission("FileReadEA", [OAA.Permission.MetadataRead]);
            AddPermission("FileWriteAttributes", [OAA.Permission.MetadataWrite]);
            AddPermission("FileWriteData", [OAA.Permission.DataWrite]);
            AddPermission("FileWriteEA", [OAA.Permission.MetadataWrite]);
            AddPermission("FullControl", [ 
                OAA.Permission.DataCreate,
                OAA.Permission.DataDelete,
                OAA.Permission.DataRead,
                OAA.Permission.DataWrite,
                OAA.Permission.MetadataCreate,
                OAA.Permission.MetadataDelete,
                OAA.Permission.MetadataRead,
                OAA.Permission.MetadataWrite
            ]);
            AddPermission("GenericAll", [
                OAA.Permission.DataCreate,
                OAA.Permission.DataDelete,
                OAA.Permission.DataRead,
                OAA.Permission.DataWrite,
                OAA.Permission.MetadataCreate,
                OAA.Permission.MetadataDelete,
                OAA.Permission.MetadataRead,
                OAA.Permission.MetadataWrite
            ]);
            AddPermission("GenericExecute", [OAA.Permission.NonData]);
            AddPermission("GenericRead", [OAA.Permission.DataRead]);
            AddPermission("GenericWrite", [OAA.Permission.DataWrite]);
            AddPermission("ListDirectory", [OAA.Permission.MetadataRead]);
            AddPermission("MaximumAllowed", [
                OAA.Permission.DataCreate,
                OAA.Permission.DataDelete,
                OAA.Permission.DataRead,
                OAA.Permission.DataWrite,
                OAA.Permission.MetadataCreate,
                OAA.Permission.MetadataDelete,
                OAA.Permission.MetadataRead,
                OAA.Permission.MetadataWrite
            ]);
            AddPermission("Modify", [OAA.Permission.DataWrite, OAA.Permission.DataDelete]);
            AddPermission("Read", [OAA.Permission.DataRead]);
            AddPermission("ReadControl", [OAA.Permission.MetadataRead]);
            AddPermission("ReadAndExecute", [OAA.Permission.DataRead]);
            AddPermission("ReadAttributes", [OAA.Permission.MetadataRead]);
            AddPermission("ReadData", [OAA.Permission.DataRead]);
            AddPermission("ReadExtendedAttributes", [OAA.Permission.DataRead]);
            AddPermission("ReadPermissions", [OAA.Permission.MetadataRead]);
            AddPermission("Synchronize", [OAA.Permission.NonData]);
            AddPermission("TakeOwnership", [OAA.Permission.MetadataWrite]);
            AddPermission("Traverse", [OAA.Permission.MetadataRead]);
            AddPermission("Write", [OAA.Permission.DataWrite]);
            AddPermission("WriteAttributes", [OAA.Permission.MetadataWrite]);
            AddPermission("WriteDac", [OAA.Permission.MetadataWrite]);
            AddPermission("WriteData", [OAA.Permission.DataWrite]);
            AddPermission("WriteExtendedAttributes", [OAA.Permission.MetadataWrite]);
            AddPermission("WriteOwner", [OAA.Permission.MetadataWrite]);
        }

        /// <summary>
        /// Generate the Veza JSON payload for the Filesystem server submission
        /// </summary>
        /// <returns>The Veza payload as a JSON string for upload</returns>
        public string ServerJSON()
        {
            if (Server is not null)
            {
                return PayloadtoJSON(ServerPayload());
            }
            else
            {
                throw new TemplateException("No server defined; cannot generate Server JSON");
            }
        }

        /// <summary>
        /// Generate a serializable Filesystem server submission
        /// </summary>
        /// <returns>The Veza payload as a dictionary</returns>
        public Dictionary<string, object> ServerPayload()
        {
            Trace.TraceInformation("Generating Filesystem Server payload");
            Dictionary<string, object> custom_properties = new()
            {
                { "server_properties", DefinedProperties[typeof(Server)].ToDictionary() },
                { "mount_properties", DefinedProperties[typeof(Mount)].ToDictionary() }
            };

            Dictionary<string, object> payload = new()
            {
                ["custom_property_definition"] = custom_properties,
                ["file_system_type"] = FilesystemType,
                ["mounts"] = from mount in Mounts.Values select mount.ToDictionary(),
                ["permissions"] = from permission in Permissions.Values select permission.ToDictionary()
            };

            if (Server is not null) 
            { 
                payload.Add("server", Server.ToDictionary());
                payload.Add("server_unique_id", Server.UniqueId);
            }

            return payload;
        }

        /// <summary>
        /// Set a custom property on the Filesystem object
        /// </summary>
        /// <param name="property_name">The string name of the property</param>
        /// <param name="property_value">The value of the property to set</param>
        /// <exception cref="TemplateException">Throws if the property to be set is not defined</exception>
        public void SetProperty(string property_name, object property_value)
        {
            Trace.TraceInformation($"Setting property {property_name} on Filesystem");
            if (DefinedProperties[typeof(Filesystem)] == null)
            {
                throw new TemplateException($"No custom properties defined; cannot set value for property {property_name}");
            }
            if (DefinedProperties[typeof(Filesystem)].ValidateProperty(property_name))
            {
                Properties.Add(property_name, property_value);
            }
            else
            {
                throw new TemplateException($"Custom property {property_name} not defined on Filesystem; cannot set value");
            }
        }
    }

}
