using Veza.OAA.Base;

namespace Veza.OAA.Filesystem
{
    /// <summary>
    /// Defines an individual permission on the Server submission that can be subsequently
    /// applied to Folder objects during folder discovery/submission
    /// </summary>
    public class Permission : VezaEntity
    {
        internal List<OAA.Permission> PermissionType { get; set; }

        public Permission(
            string name,
            List<OAA.Permission> permissions,
            string? description = null,
            Dictionary<string, object>? properties = null,
            PropertyDefinitions? propertyDefinitions = null,
            List<Tag>? tags = null
        ) : base(
            name: name,
            entityType: typeof(Permission),
            description: description,
            properties: properties,
            propertyDefinitions: propertyDefinitions,
            tags: tags
        )
        {
            UniqueId = null;
            PermissionType = permissions;
        }

        /// <summary>
        /// Convert the Permission object to a Dictionary for serialization
        /// </summary>
        /// <returns>The Dictionary representation of the object</returns>
        internal Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new()
            {
                ["name"] = Name,
                ["permission_type"] = from p in PermissionType select p.ToString()
            };
            if (!string.IsNullOrEmpty(Description)) { payload.Add("description", Description); }
            if (Tags.Any()) { payload.Add("tags", (from t in Tags select t.ToDictionary()).ToList()); }

            return payload;
        }
    }
}
