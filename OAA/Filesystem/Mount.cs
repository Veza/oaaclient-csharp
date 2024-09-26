using System.Diagnostics;
using Veza.OAA.Base;

namespace Veza.OAA.Filesystem
{
    /// <summary>
    /// Defines a filesystem share "root" 
    /// 
    /// The Mount object is the top-level directory under which discovered folders
    /// exist.
    /// </summary>
    public class Mount : VezaEntity
    {
        internal DateTime? CreatedAt { get; set; }
        internal Dictionary<string, List<string>> IdentityToPermissions { get; set; }
        internal DateTime? UpdatedAt { get; set; }

        public Mount(
            string name,
            DateTime? createdAt = null,
            string? description = null,
            Dictionary<string, object>? properties = null,
            PropertyDefinitions? propertyDefinitions = null,
            List<Tag>? tags = null,
            string? uniqueId = null,
            DateTime? updatedAt = null
        ) : base(
            name: name,
            entityType: typeof(Mount),
            description: description,
            properties: properties,
            propertyDefinitions: propertyDefinitions,
            tags: tags,
            uniqueId: uniqueId
        )
        {
            CreatedAt = createdAt;
            IdentityToPermissions = [];
            UpdatedAt = updatedAt;
        }

        /// <summary>
        /// Add a single permission to an identity
        /// </summary>
        /// <param name="userName">The string user name to which the permission is assigned</param>
        /// <param name="permission">The string permission being assigned</param>
        internal void AddPermissions(string userName, string permission)
        {
            Debug.WriteLine($"Adding permission {permission} for {userName} on Mount {Name}");
            if (IdentityToPermissions.ContainsKey(userName))
            {
                if (!IdentityToPermissions[userName].Contains(permission))
                {
                    IdentityToPermissions[userName].Add(permission);
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
        internal void AddPermissions(string userName, List<string> permissions)
        {
            Debug.WriteLine($"Adding permissions for {userName} on Mount {Name}");
            if (IdentityToPermissions.ContainsKey(userName))
            {
                IdentityToPermissions[userName] =
                    IdentityToPermissions[userName].Union(permissions).ToList();
            }
            else
            {
                IdentityToPermissions[userName] = permissions;
            }
        }

        /// <summary>
        /// Convert the Mount object to a Dictionary for payload serialization
        /// </summary>
        /// <returns>The Dictionary representation of the object</returns>
        internal Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new()
            {
                ["name"] = Name,
                ["unique_id"] = UniqueId
            };
            if (CreatedAt is not null) { payload.Add("created_at", CreatedAt.ToRFC3339()); }
            if (!string.IsNullOrEmpty(Description)) { payload.Add("description", Description); }
            if (Properties.Any()) { payload.Add("custom_properties", Properties); }
            if (UpdatedAt is not null) { payload.Add("updated_at", UpdatedAt.ToRFC3339()); }
            if (IdentityToPermissions.Any()) { payload.Add("identity_to_permissions", IdentityToPermissions); }
            if (Tags.Any()) { payload.Add("tags", (from t in Tags select t.ToDictionary()).ToList()); }

            return payload;
        }
    }
}
