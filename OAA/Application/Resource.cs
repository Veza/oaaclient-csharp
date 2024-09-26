using Veza.OAA.Exceptions;
using Veza.OAA.Base;

namespace Veza.OAA.Application
{
    /// <summary>
    /// Resources and subresources. 
    /// 
    /// Used for representing components of the application to which authorization is granted
    /// 
    /// Each resource has a name and a type; the type can be used for grouping and filtering
    /// </summary>
    public class Resource : VezaEntity
    {
        internal string ApplicationName { get; set; }
        internal List<Dictionary<string, string>> Connections { get; set; }
        internal string Key { get; set; }
        internal string ResourceType { get; set; }
        internal Dictionary<string, Resource> SubResources { get; set; }

        internal Resource(
            string name,
            string applicationName,
            string resourceType,
            string? description = null,
            PropertyDefinitions? propertyDefinitions = null,
            string? uniqueId = null,
            string? key = null
        ) : base(
            name: name,
            entityType: typeof(Resource),
            description: description,
            propertyDefinitions: propertyDefinitions,
            uniqueId: uniqueId
        )
        {
            ApplicationName = applicationName;
            Connections = [];
            Key = key ?? uniqueId ?? name;
            ResourceType = resourceType;
            SubResources = [];
        }

        /// <summary>
        /// Add an external connection to the Resource
        /// 
        /// Used to add a relationship to another entity discovered by Veza such as a service account or AWS IAM role
        /// </summary>
        /// <param name="id">Unique identifier for the connection entity</param>
        /// <param name="nodeType">The string type of the connecting node</param>
        internal void AddConnection(string id, string nodeType)
        {
            Dictionary<string, string> connection = new()
            {
                { "id", id },
                { "node_type", nodeType}
            };

            if (!Connections.Contains(connection))
            {
                Connections.Add(connection);
            }
        }

        /// <summary>
        /// Create a new sub-resource under the current resource
        /// </summary>
        /// <param name="name">The name of the subresource</param>
        /// <param name="description">Optional string description of the subresource</param>
        /// <param name="resourceType">The string type of the subresource</param>
        /// <param name="unique_id">An optional unique identifier for the subresource</param>
        /// <returns>
        /// The newly created subresource
        /// </returns>
        internal Resource AddSubResource(string name, string resourceType, string? description = null, string? uniqueId = null)
        {
            string identifier = uniqueId ?? name;

            if (SubResources.ContainsKey(identifier))
            {
                throw new TemplateException($"Cannot add subresource {identifier} - unique identifier already exists.");
            }

            Resource subresource = new(
                name: name,
                description: description,
                applicationName: ApplicationName,
                propertyDefinitions: PropertyDefinitions,
                resourceType: resourceType,
                key: $"{Key}.{identifier}",
                uniqueId: uniqueId
            );

            SubResources.Add(identifier, subresource);
            return subresource;
        }

        /// <summary>
        /// Return a dictionary representation of the Resource
        /// </summary>
        /// <returns>
        /// A dictionary representation of the Resource
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new()
            {
                { "name", Name },
                { "resource_type", ResourceType },
            };

            if (Connections.Any())
                payload.Add("connections", Connections);

            if (!string.IsNullOrEmpty(Description))
                payload.Add("description", Description);

            if (Properties.Any())
                payload.Add("custom_properties", Properties);

            if (!string.IsNullOrEmpty(UniqueId))
                payload.Add("id", UniqueId);


            if (SubResources.Any())
            {
                List<Dictionary<string, object>> subResources = [];
                foreach (Resource subresource in SubResources.Values)
                {
                    subResources.Add(subresource.ToDictionary());
                }
                payload.Add("sub_resources", subResources);
            }

            if (Tags.Any())
                payload.Add("tags", Tags);

            return payload;
        }

        /// <summary>
        /// Return a string representation of the Resource
        /// </summary>
        /// <returns>
        /// A string representation of the Resource
        /// </returns>
        public override string ToString()
        { 
            return $"Resource: {Name} ({UniqueId}) - {ResourceType}"; 
        }

    }
}
