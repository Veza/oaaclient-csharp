using System.Diagnostics;
using Veza.OAA.Base;

namespace Veza.OAA.Filesystem
{
    /// <summary>
    /// Defines a unique filesystem path, its properties, and permissions assignments
    /// </summary>
    public class Folder : VezaEntity
    {
        public DateTime? CreatedAt { get; set; }
        public Dictionary<string, List<string>> IdentityToPermissions { get; set; }
        public bool InheritParentPermissions { get; set; }
        public string ParentId { get; set; }
        public string Path { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Folder(
            string name,
            string parentId,
            string path,
            DateTime? createdAt = null,
            string? description = null,
            bool inheritParentPermissions = false,
            Dictionary<string, object>? properties = null,
            PropertyDefinitions? propertyDefinitions = null,
            List<Tag>? tags = null,
            string? uniqueId = null,
            DateTime? updatedAt = null
        ) : base(
            name: name,
            entityType: typeof(Folder),
            description: description,
            properties: properties,
            propertyDefinitions: propertyDefinitions,
            tags: tags,
            uniqueId: uniqueId
        )
        {
            CreatedAt = createdAt;
            IdentityToPermissions = [];
            InheritParentPermissions = inheritParentPermissions;
            ParentId = parentId;
            Path = path;
            UpdatedAt = updatedAt;
        }

        /// <summary>
        /// Add a single permission to an identity
        /// </summary>
        /// <param name="userName">The string user name to which the permission is assigned</param>
        /// <param name="permission">The string permission being assigned</param>
        public void AddPermissions(string userName, string permission)
        {
            Trace.WriteLine($"Adding permission {permission} for user {userName} on {Path}");
            if (IdentityToPermissions.TryGetValue(userName, out List<string>? value))
            {
                if (!value.Contains(permission))
                {
                    value.Add(permission);
                }
            }
            else
            {
                IdentityToPermissions.Add(userName, [permission]);
            }
        }

        /// <summary>
        /// Add a list of permissions to an identity
        /// </summary>
        /// <param name="userName">The string user name to which the permissions are assigned</param>
        /// <param name="permissions">A string list of the permissions being assigned</param>
        public void AddPermissions(string userName, List<string> permissions)
        {
            Trace.WriteLine($"Adding permissions for user {userName} on {Path}");
            if (IdentityToPermissions.TryGetValue(userName, out List<string>? value))
            {
                IdentityToPermissions[userName] = value.Union(permissions).ToList();
            }
            else
            {
                IdentityToPermissions[userName] = permissions;
            }
        }

        /// <summary>
        /// Convert the Folder object to a Dictionary for payload serialization
        /// </summary>
        /// <returns>The Dictionary representation of the object</returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new()
            {
                ["inherit_parent_permissions"] = InheritParentPermissions,
                ["name"] = Name,
                ["parent_id"] = ParentId,
                ["path"] = Path,
                ["unique_id"] = UniqueId,
            };
            if (CreatedAt is not null) { payload.Add("created_at", CreatedAt.ToRFC3339()); }
            if (!string.IsNullOrEmpty(Description)) { payload["description"] = Description; }
            if (Properties.Count != 0) { payload.Add("custom_properties", Properties); }
            if (UpdatedAt is not null) { payload.Add("updated_at", UpdatedAt.ToRFC3339()); }
            if (IdentityToPermissions.Count != 0) { payload.Add("identity_to_permissions", IdentityToPermissions); }
            if (Tags.Count != 0) { payload.Add("tags", (from t in Tags select t.ToDictionary()).ToList()); }

            return payload;
        }
    }
}
