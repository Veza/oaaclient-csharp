using System.Diagnostics;
using System.Text.Json;
using Veza.OAA.Base;
using Veza.OAA.Exceptions;

namespace Veza.OAA.Application
{
    /// <summary>
    /// Class for modeling application authorization using the OAA Application template.
    /// 
    /// The CustomApplication class consists of identities, resources, and permissions and produces
    /// the OAA JSON payload for the custom application template.
    /// 
    /// This class uses dictionaries to track most entities that can be referenced after creation.
    /// Dictionary keys are case-insensitive identitifiers.
    /// </summary>
    public class CustomApplication : Application
    {
        public Dictionary<string, CustomPermission> CustomPermissions { get; set; }
        public Dictionary<Type, PropertyDefinitions> DefinedProperties { get; set; }
        public Dictionary<string, Group> Groups { get; set; }
        public Dictionary<string, Permission> IdentityToPermissions { get; set; }
        public Dictionary<string, IdPIdentity> IdPIdentities { get; set; }
        public Dictionary<string, PropertyDefinitions> ResourceProperties { get; set; }
        public Dictionary<string, Role> Roles { get; set; }
        public Dictionary<string, Resource> Resources { get; set; }
        public Dictionary<string, User> Users { get; set; }

        public CustomApplication(
            string applicationType,
            string name,
            string? description = null) :
            base(
                name: name,
                applicationType: applicationType,
                description: description
            )
        {
            DefinedProperties = new Dictionary<Type, PropertyDefinitions>()
            {
                { typeof(CustomApplication), new PropertyDefinitions() },
                { typeof(Group), new PropertyDefinitions() },
                { typeof(Role), new PropertyDefinitions() },
                { typeof(User), new PropertyDefinitions() }
            };
            Groups = new Dictionary<string, Group>(StringComparer.OrdinalIgnoreCase);
            IdentityToPermissions = new Dictionary<string, Permission>(StringComparer.OrdinalIgnoreCase);
            IdPIdentities = new Dictionary<string, IdPIdentity>(StringComparer.OrdinalIgnoreCase);
            CustomPermissions = new Dictionary<string, CustomPermission>(StringComparer.OrdinalIgnoreCase);
            ResourceProperties = [];
            Resources = new Dictionary<string, Resource>(StringComparer.OrdinalIgnoreCase);
            Roles = new Dictionary<string, Role>(StringComparer.OrdinalIgnoreCase);
            Users = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Create a new custom permission and add it to the CustomApplication
        /// </summary>
        /// <param name="name">The name of the permission</param>
        /// <param name="permission">The canonical permission that the custom permission reprsents</param>
        /// <param name="applyToSubResources">If true, when the permission is applied to the application or resource, identity also has permissions to all children</param>
        /// <param name="resourceTypes">List of string resource types that the permission applies to</param>
        /// <returns>The newly created CustomPermission object</returns>
        public CustomPermission AddCustomPermission(string name, Permission permission, bool applyToSubResources = false, List<string>? resourceTypes = null)
        {
            AddCustomPermission(name: name, permissions: [permission], 
                                applyToSubResources: applyToSubResources, resourceTypes: resourceTypes);
            return CustomPermissions[name];
        }

        /// <summary>
        /// Create a new custom permission and add it to the CustomApplication
        /// </summary>
        /// <param name="name">The name of the permission</param>
        /// <param name="permissions">The list of canonical permissions that the custom permission represents</param>
        /// <param name="applyToSubResources">If true, when the permission is applied to the application or resource, identity also has permissions to all children</param>
        /// <param name="resourceTypes">List of string resource types that the permission applies to</param>
        /// <returns>
        /// The newly created CustomPermission object
        /// </returns>
        /// <exception cref="TemplateException">
        /// Throws if the Custom Permission already exists
        /// </exception>
        public CustomPermission AddCustomPermission(string name, List<Permission> permissions, bool applyToSubResources = false, List<string>? resourceTypes = null)
        {
            if (CustomPermissions.ContainsKey(name)) 
            { 
                throw new TemplateException($"Custom permission {name} already exists"); 
            }

            Debug.WriteLine($"Adding CustomPermission {name}");
            CustomPermissions[name] = new CustomPermission(
                name: name,
                permissions: permissions,
                applyToSubResources: applyToSubResources,
                resourceTypes: resourceTypes
            );

            return CustomPermissions[name];
        }

        /// <summary>
        /// Create a new local group in the CustomApplication
        /// 
        /// Groups can be associated to resources via direct permissions or roles
        /// All users in the Group are granted the group's authorization
        /// 
        /// Local groups will be identified by `name` by default; if `unique_id` is provided, it will be used as the identifier instead
        /// </summary>
        /// <param name="name">Display name for the group</param>
        /// <param name="identities">A string list of identities (usually e-mail addresses) for the local group</param>
        /// <param name="uniqueId">A unique identifier for the group</param>
        /// <returns>
        /// The newly created Group object
        /// </returns>
        /// <exception cref="TemplateException">
        /// Throws if the group already exists
        /// </exception>
        public Group AddGroup(string name, List<string>? identities = null, string? uniqueId = null)
        {
            string identifier = uniqueId ?? name;

            if (Groups.ContainsKey(identifier))
            {
                throw new TemplateException($"Local Group {identifier} already exists");
            }

            Debug.WriteLine($"Adding Group {name}");
            Groups[identifier] = new Group(
                name: name, 
                identities: identities, 
                uniqueId: uniqueId, 
                propertyDefinitions: DefinedProperties[typeof(Group)]
            );
            return Groups[identifier];
        }

        /// <summary>
        /// Create an IdP principal identity in the CustomApplication
        /// 
        /// IdP users and groups can be authorized directly to applications and resources by associating custom application
        /// permissions and roles with an IdP identity's name or email
        /// </summary>
        /// <param name="name">IdP unique identifier for the user or group</param>
        /// <returns>
        /// The newly created IdPIdentity object
        /// </returns>
        /// <exception cref="TemplateException">
        /// Throws if the IdP Identity already exists
        /// </exception>
        public IdPIdentity AddIdPIdentity(string name)
        {
            if (IdPIdentities.ContainsKey(name)) 
            { 
                throw new TemplateException($"IdP Identity {name} already defined"); 
            }

            Debug.WriteLine($"Adding IdPIdentity {name}");
            IdPIdentities[name] = new IdPIdentity(name: name);
            return IdPIdentities[name];
        }

        /// <summary>
        /// Create a new role in the CustomApplication
        /// 
        /// A Role represents a collection of permissions
        /// Identities can be assigned a role on the CustomApplication or on a resource
        /// Local roles will be identified by `name` by default; if `unique_id` is provided, it will be used as the identifier instead
        /// 
        /// When a permission that has `resource_types` is added to a Role, it will only apply to resources with a matching `resource_type`
        /// </summary>
        /// <param name="name">Display name for the role</param>
        /// <param name="permissions">List of CustomPermission names to include in the role. CustomPermissions must be created separately</param>
        /// <param name="uniqueId">A unique identifier for the role</param>
        /// <returns>
        /// The newly created role
        /// </returns>
        /// <exception cref="TemplateException">
        /// Throws if the role already exists
        /// </exception>
        public Role AddRole(string name, List<string>? permissions = null, string? uniqueId = null)
        {
            string identifier = uniqueId ?? name;
            if (Roles.ContainsKey(identifier))
            {
                throw new TemplateException($"Local Role {identifier} already exists");
            }

            Debug.WriteLine($"Adding Role {name}");
            Roles[identifier] = new Role(
                name: name, 
                permissions: permissions, 
                roles: null, 
                tags: null, 
                uniqueId: uniqueId, 
                propertyDefinitions: DefinedProperties[typeof(Role)]
            );
            return Roles[identifier];
        }

        /// <summary>
        /// Create a new resource in the CustomApplication
        /// 
        /// Resource type is used to group and filter application resources.
        /// Resource is identified by `name` unless `unique_id` is provided. `name` must be unique if `unique_id` is not present.
        /// </summary>
        /// <param name="name">The name of the resource</param>
        /// <param name="resourceType">The type of the resource</param>
        /// <param name="description">Description of the resource</param>
        /// <param name="uniqueId">A unique identifier for the resource</param>
        /// <returns>
        /// The newly created Resource object
        /// </returns>
        /// <exception cref="TemplateException">
        /// Throws if the resource already exists
        /// </exception>
        public Resource AddResource(string name, string resourceType, string? description = null, string? uniqueId = null)
        {
            string identifier = uniqueId ?? name;
            if (Resources.ContainsKey(identifier))
            {
                throw new TemplateException($"Resource {identifier} already exists");
            }

            PropertyDefinitions propertyDefinitions;
            if (ResourceProperties.TryGetValue(resourceType, out PropertyDefinitions? value)) 
            {
                propertyDefinitions = value;
            }
            else
            {
                propertyDefinitions = new PropertyDefinitions();
            }

            Trace.WriteLine($"Adding Resource {name}");
            Resources[identifier] = new Resource(
                name: name,
                description: description,
                applicationName: Name,
                propertyDefinitions: propertyDefinitions,
                resourceType: resourceType,
                uniqueId: uniqueId
            );

            return Resources[identifier];
        }

        /// <summary>
        /// Create a new local user for the CustomApplication
        /// 
        /// Local users can be assigned to groups and associated with resources via direct permissions or roles
        /// Groups and identities can be preovided at creation or added later
        /// 
        /// Local users will be identified by `name` by default; if `unique_id` is provided, it will be used as the identifier instead
        /// 
        /// </summary>
        /// <param name="name">Display name for the user</param>
        /// <param name="identities">A string list of identities (usually e-mail addresses) for the local user. Used to map the user to IdP identities</param>
        /// <param name="groups">A list of string group names to which the user will be added</param>
        /// <param name="uniqueId">A unique identifier for the user</param>
        /// <returns>
        /// The newly created user
        /// </returns>
        public User AddUser(string name, List<string>? identities = null, List<string>? groups = null, string? uniqueId = null)
        {
            string identifier = uniqueId ?? name;
            if (Users.ContainsKey(identifier))
            {
                throw new TemplateException($"Local User {identifier} already exists");
            }

            Debug.WriteLine($"Adding User {name}");
            Users[identifier] = new User(
                name: name,
                identities: identities,
                groups: groups, 
                uniqueId: uniqueId,
                propertyDefinitions: DefinedProperties[typeof(User)]
            );
            return Users[identifier];
        }

        /// <summary>
        /// Define a property for a resource type
        /// </summary>
        /// <param name="propertyName">The string name of the property</param>
        /// <param name="propertyType">The type of the property</param>
        /// <param name="resourceType">The string type of the resource</param>
        public void DefineResourceProperty(string propertyName, Type propertyType, string resourceType)
        {
            if (!ResourceProperties.TryGetValue(resourceType, out PropertyDefinitions? value))
            {
                Trace.TraceWarning($"Resource type {resourceType} is not defined");
                value = new PropertyDefinitions();
                ResourceProperties.Add(resourceType, value);
            }
            PropertyDefinitions.ValidatePropertyName(propertyName);
            value.DefineProperty(propertyName, propertyType);
        }

        /// <summary>
        /// Return the "permissions" section of the payload as a serializable dictionary
        /// </summary>
        /// <returns>
        /// The "permissions" section of the OAA payload as a serializable dictionary
        /// </returns>
        public List<Dictionary<string, object>> GetCustomPermissions()
        {
            List<Dictionary<string, object>> payload = (from p in CustomPermissions.Values select p.ToDictionary()).ToList();
            return payload;
        }

        /// <summary>
        /// Collect authorizations for all identities into a single list of dictionaries
        /// </summary>
        /// <returns>
        /// A list of dictionaries containing all identity/permissions authorizations
        /// </returns>
        public List<Dictionary<string, object>> GetIdentityToPermissions()
        {
            List<Dictionary<string, object>> identitiesToPermissions = [];
            List<Identity> identities = [.. Groups.Values, .. IdPIdentities.Values, .. Users.Values];
            foreach (Identity identity in identities)
            {
                Dictionary<string, object> entry = identity.GetIdentityToPermissions(applicationName: Name);
                if (entry.ContainsKey("application_permissions") || entry.ContainsKey("role_assignments"))
                { 
                    identitiesToPermissions.Add(entry); 
                }
            }

            return identitiesToPermissions;
        }

        /// <summary>
        /// Get the Veza payload dictionary
        /// 
        /// Returns the complete OAA template payload for the CustomApplication as a serializable dictionary
        /// </summary>
        /// <returns>
        /// The OAA payload as a dictionary
        /// </returns>
        public Dictionary<string, object> GetPayload()
        {
            List<Dictionary<string, object>> resourceProperties = [];
            foreach(string r in ResourceProperties.Keys)
            {
                resourceProperties.Add(new Dictionary<string, object>()
                {
                    {"resource_type", r },
                    {"properties", ResourceProperties[r].ToDictionary()}
                });
            }

            Dictionary<string, object> customProperties = new()
            {
                { "application_properties", DefinedProperties[typeof(CustomApplication)].ToDictionary() },
                { "application_type", ApplicationType },
                { "local_group_properties", DefinedProperties[typeof(Group)].ToDictionary() },
                { "local_role_properties", DefinedProperties[typeof(Role)].ToDictionary() },
                { "local_user_properties", DefinedProperties[typeof(User)].ToDictionary() },
                { "resources", resourceProperties }
            };

            Dictionary<string, object> payload = new()
            {
                ["applications"] = new List<Dictionary<string, object?>>() { ToDictionary() },
                ["custom_property_definition"] = new Dictionary<string, object> 
                {
                    { "applications", new List<Dictionary<string, object>> ()
                        {
                            customProperties
                        }
                    }
                },
                ["identity_to_permissions"] = GetIdentityToPermissions(),
                ["permissions"] = GetCustomPermissions()
            };

            return payload;
        }

        /// <summary>
        /// Get the Veza payload JSON
        /// </summary>
        /// <returns>
        /// The OAA payload as a JSON string
        /// </returns>
        public string GetJSONPayload()
        {
            JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
            return JsonSerializer.Serialize(GetPayload(), jsonOptions);
        }

        /// <summary>
        /// Set a custom property on the CustomAplication object
        /// </summary>
        /// <param name="name">The string name of the property</param>
        /// <param name="value">The value of the property to set</param>
        /// <exception cref="TemplateException">Throws if the property to be set is not defined</exception>
        public new void SetProperty(string name, object value)
        {
            if (DefinedProperties[typeof(CustomApplication)] == null)
            {
                throw new TemplateException($"No custom properties defined; cannot set property");
            }
            if (DefinedProperties[typeof(CustomApplication)].ValidateProperty(name))
            {
                Properties.Add(name, value);
            }
            else
            {
                throw new TemplateException($"Custom property {name} not defined on Custom Application");
            }
        }

        /// <summary>
        /// Return the "applications" section of the payload as a serializable dictionary
        /// </summary>
        /// <returns>
        /// The "applications" section of the OAA payload as a serializable dictionary
        /// </returns>
        public Dictionary<string, object?> ToDictionary()
        {
            Dictionary<string, object?> payload = new()
            {
                ["application_type"] = ApplicationType,
                ["custom_properties"] = Properties,
                ["description"] = Description,
                ["local_groups"] = (from g in Groups.Values select g.ToDictionary()).ToList(),
                ["local_roles"] = (from r in Roles.Values select r.ToDictionary()).ToList(),
                ["local_users"] = (from u in Users.Values select u.ToDictionary()).ToList(),
                ["name"] = Name,
                ["resources"] = (from r in Resources.Values select r.ToDictionary()).ToList(),
                ["tags"] = (from t in Tags select t.ToDictionary()).ToList()
            };

            return payload;
        }

        /// <summary>
        /// Return a string representation of the CustomApplication
        /// </summary>
        /// <returns>
        /// A string representation of the CustomApplication
        /// </returns>
        public override string ToString()
        {
            return $"Custom Application {Name} - {ApplicationType}";
        }
    }
}
