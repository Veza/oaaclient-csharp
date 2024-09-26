using Veza.OAA.Base;
using Veza.OAA.Exceptions;

namespace Veza.OAA.Application
{
    /// <summary>
    /// Base class for deriving all identity types.
    /// 
    /// TODO: groups and identities as dict?
    /// </summary>
    public abstract class Identity : VezaEntity
    {
        public List<string> ApplicationPermissions { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<string> Groups { get; set; }
        public List<string> Identities { get; set; }
        public IdentityType IdentityType { get; set; }
        public Dictionary<string, List<string>> ResourcePermissions { get; set; }
        public Dictionary<Role, List<string>>? ResourceRoles { get; set; }
        public Dictionary<string, Dictionary<string, object>> RoleAssignments { get; set; }

        public Identity(
            string name,
            IdentityType identityType,
            List<string>? groups = null,
            List<string>? identities = null,
            PropertyDefinitions? propertyDefinitions = null,
            string? uniqueId = null
        ) : base(
            name: name,
            entityType: typeof(Identity),
            propertyDefinitions: propertyDefinitions,
            uniqueId: uniqueId
        )
        {
            ApplicationPermissions = [];
            Identities = identities ?? [];
            Groups = groups ?? [];
            IdentityType = identityType;
            ResourcePermissions = [];
            RoleAssignments = [];
        }

        /// <summary>
        /// Add identity to a Group
        /// 
        /// Note: The Group must be created separately before calling this method
        /// </summary>
        /// <param name="groupName">The string name of the Group</param>
        public void AddGroup(string groupName)
        {
            if (this is Group && Name == groupName)
            {
                throw new TemplateException("Cannot add group to itself as a subgroup");
            }
            if (!Groups.Contains(groupName))
            {
                Groups.Add(groupName);
            }
        }

        /// <summary>
        /// Add an identity to the identity
        /// Identity should match the email address or other principal identifier for the IdP user;
        /// </summary>
        /// <param name="identity">Email or identifier for the IdP user</param>
        public void AddIdentity(string identity)
        {
            Identities.Add(identity);
        }

        /// <summary>
        /// Add multiple identities to the User
        /// </summary>
        /// <param name="identities">A list of identity strings to add to the User</param>
        public void AddIdentities(List<string> identities)
        {
            Identities.AddRange(identities);
        }

        /// <summary>
        /// Adds permission to an entity that applies to application resources
        /// </summary>
        /// <param name="permission">String permission name</param>
        /// <param name="resources">List of resources on which to apply the permission</param>
        /// 
        public void AddPermission(string permission, List<Resource>? resources = null, bool applyToApplication = false)
        {
            if (resources == null && (applyToApplication == false))
            {
                throw new TemplateException("Must add permission to either resources or application; resources cannot be null while applyToApplication is false");
            }

            if (applyToApplication && !ApplicationPermissions.Contains(permission))
            {
                ApplicationPermissions.Add(permission);
            }

            if (resources != null)
            {
                if (ResourcePermissions.ContainsKey(permission))
                {
                    foreach (Resource r in resources)
                    {
                        if (!ResourcePermissions[permission].Contains(r.Key))
                        {
                            ResourcePermissions[permission].Add(r.Key);
                        }
                    }
                }
                else
                {
                    ResourcePermissions[permission] = (from r in resources select r.Key).ToList();
                }
            }
        }

        /// <summary>
        /// Add a role to an identity
        /// 
        /// The role authorizes the identity to either the application or an application resource
        /// </summary>
        /// <param name="name">The string name of the role</param>
        /// <param name="resources">The list of custom resources the role applies to; if null, the role applies to the application</param>
        /// <param name="applyToApplication">Indicates that the role applies to the entire application</param>
        public void AddRole(string name, List<Resource>? resources = null, bool? applyToApplication = null)
        {
            List<string> resource_keys = [];
            if (resources != null)
            {
                resource_keys = (from r in resources select r.Key).ToList();
            }

            if (!RoleAssignments.ContainsKey(name))
            {
                RoleAssignments.Add(name, new Dictionary<string, object>
                {
                    {"apply_to_application", applyToApplication ?? false },
                    {"resources", resource_keys }
                });
            }
            else
            {
                if (applyToApplication != null)
                {
                    RoleAssignments[name]["apply_to_application"] = applyToApplication;
                }
                List<string> existing_keys = (List<string>)RoleAssignments[name]["resources"];
                existing_keys.AddRange(resource_keys);
                RoleAssignments[name]["resources"] = existing_keys;
            }
        }

        /// <summary>
        /// Return a dictionary of all the identitiy's permissions and roles
        /// 
        /// Formats the identity's permissions and roles for the CustomApplication template payload
        /// </summary>
        /// <param name="applicationName">The string application name</param>
        /// <returns>
        /// A dictionary of all the identity's permissions and roles
        /// </returns>
        public virtual Dictionary<string, object> GetIdentityToPermissions(string applicationName)
        {
            string identifier = UniqueId ?? Name;

            Dictionary<string, object> payload = new()
            {
                { "identity", identifier },
                { "identity_type", IdentityType.ToString() }
            };

            List<Dictionary<string, object>> application_permissions = [];
            foreach (string permission in ApplicationPermissions)
            {
                application_permissions.Add(new Dictionary<string, object>
                {
                    { "application", applicationName },
                    { "permission", permission },
                    { "apply_to_application", true }
                });
            };

            foreach (string permission in ResourcePermissions.Keys)
            {
                application_permissions.Add(new Dictionary<string, object>
                {
                    { "application", applicationName },
                    { "resources", ResourcePermissions[permission] },
                    { "permission", permission }
                 });
            };

            List<Dictionary<string, object>> role_assignments = [];
            foreach (string role in RoleAssignments.Keys)
            {
                role_assignments.Add(new Dictionary<string, object>
                {
                    {"application", applicationName },
                    {"role", role },
                    {"apply_to_application", RoleAssignments[role]["apply_to_application"] },
                    {"resources", RoleAssignments[role]["resources"] }
                });
            };

            if (application_permissions.Any()) { payload["application_permissions"] = application_permissions; }
            if (role_assignments.Any()) { payload["role_assignments"] = role_assignments; }

            return payload;
        }
    }
}
