using System.Diagnostics;

namespace Veza.OAA.Application
{
    /// <summary>
    /// Defines Custom Permissions for CustomApplication objects.
    /// 
    /// Custom permissions represent the named permissions for the application in its own terms (e.g. "Admin" or "PUSH")
    /// and define the veza canonical permissions that they map to.
    /// 
    /// A permission can either be applied directly to a application/resource or assigned as part of a role.
    /// 
    /// Optionally, when permissions are used as part of a role, if the `resourceTypes` list is populated, the permission
    /// will only be applied to resources whose type is in the `resourceTypes` list.
    /// </summary>
    public class CustomPermission
    {
        public bool ApplyToSubResources { get; set; }
        public string Name { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<string> ResourceTypes { get; set; }

        public CustomPermission(string name, 
                                List<Permission> permissions, 
                                bool applyToSubResources = false, 
                                List<string>? resourceTypes = null)
        {
            ApplyToSubResources = applyToSubResources;
            Name = name;
            Permissions = permissions;
            ResourceTypes = resourceTypes ?? [];
        }

        /// <summary>
        /// Add a resource type to the ResourceTypes list
        ///  
        /// Extends the list of resource types that the permission applies to when used in role assignment
        /// </summary>
        /// <param name="resourceType">The string name of the resource type</param>
        public void AddResourceType(string resourceType)
        {
            if (ResourceTypes.Contains(resourceType))
            { 
                Trace.TraceInformation($"CustomPermission already contains resource type {resourceType}"); 
            }
            else
            { 
                ResourceTypes.Add(resourceType); 
            }
        }

        /// <summary>
        /// Return a dictionary representation of the CustomPermission
        /// </summary>
        /// <returns>
        /// A dictionary representation of the CustomPermission
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new()
            {
                { "name", Name },
                { "permission_type", (from p in Permissions select p.ToString()).ToList() },
                { "apply_to_sub_resources", ApplyToSubResources },
                { "resource_types", ResourceTypes }
            };

            return payload;
        }

        /// <summary>
        /// Return a string representation of the CustomPermission
        /// </summary>
        /// <returns>
        /// A string representation of the CustomPermission
        /// </returns>
        public override string ToString()
        {
            return $"Custom Permission {Name} - {string.Join(",", Permissions)}";
        }
    }
}
