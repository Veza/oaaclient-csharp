using Veza.OAA.Base;
using Veza.OAA.Exceptions;

namespace Veza.OAA.Application
{
    /// <summary>
    /// Custom Application Role.
    /// 
    /// Local Roles are a collection of permissions (CustomPermission).
    /// Roles can be used to associate a User, Group, or IdP identity to an application, resource, or subresource.
    /// </summary>
    public class Role : VezaEntity
    {
        public List<string> Permissions { get; set; }
        public List<string> Roles { get; set; }

        public Role(
            string name,
            List<string>? permissions = null,
            List<string>? roles = null,
            List<Tag>? tags = null,
            string? uniqueId = null,
            PropertyDefinitions? propertyDefinitions = null
        ) : base(
            name: name,
            entityType: typeof(Role),
            propertyDefinitions: propertyDefinitions,
            tags: tags,
            uniqueId: uniqueId
        )
        {
            Permissions = permissions ?? [];
            Roles = roles ?? [];
        }

        /// <summary>
        /// Add a single permission to the role
        /// </summary>
        /// <param name="permission">A string permission to add to the role</param>
        /// <returns>
        /// The updated role permissions
        /// </returns>
        public List<string> AddPermission(string permission)
        {
            Permissions.Add(permission);
            return Permissions;
        }

        /// <summary>
        /// Add a list of permissions to the role
        /// </summary>
        /// <param name="permissions">A list of string permissions to add to the role</param>
        /// <returns>
        /// The updated role permissions
        /// </returns>
        public List<string> AddPermissions(List<string> permissions)
        {
            Permissions.AddRange(permissions);
            return Permissions;
        }

        /// <summary>
        /// Add a nested sub-role to the role.
        /// Nested role must be created separately
        /// </summary>
        /// <param name="role">The string name of the role to nest inside this role</param>
        /// <returns>
        /// The updated list of nested roles
        /// </returns>
        public List<string> AddRole(string role)
        {
            if (Roles.Contains(role))
            {
                throw new TemplateException($"Subrole {role} already exists in Role {Name}");
            }

            Roles.Add(role);
            return Roles;
        }

        /// <summary>
        /// Add an existing nested sub-role to the role
        /// </summary>
        /// <param name="role">The Role object to add as a sub-role</param>
        /// <returns>
        /// The updated list of nested roles
        /// </returns>
        public List<string> AddRole(Role role)
        {
            string identifier = role.UniqueId ?? role.Name;
            if (Roles.Contains(identifier))
            {
                throw new TemplateException($"Subrole {role.Name} already exists in Role {Name}");
            }

            Roles.Add(identifier);
            return Roles;
        }

        /// <summary>
        /// Return a dictionary representation of the Role
        /// </summary>
        /// <returns>
        /// A dictionary representation of the Role
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new()
            {
                { "name", Name },
                { "permissions", Permissions },
                { "tags", Tags },
                { "roles", Roles },
                { "custom_properties", Properties }
            };

            if (!string.IsNullOrEmpty(UniqueId))
                payload.Add("id", UniqueId);

            return payload;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(UniqueId))
            {
                return $"Role {Name}";
            }
            else
            {
                return $"Role {Name} ({UniqueId})";
            }

        }
    }
}
