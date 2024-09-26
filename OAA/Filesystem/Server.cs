using Veza.OAA.Base;

namespace Veza.OAA.Filesystem
{
    /// <summary>
    /// Defines a filesystem server
    /// 
    /// The Server class can contain multiple Mount objects, each of which defines the 
    /// root of a filesystem share to be discovered
    /// </summary>
    public class Server : VezaEntity
    {
        public Server(
            string name,
            string? description = null,
            Dictionary<string, object>? properties = null,
            PropertyDefinitions? propertyDefinitions = null,
            List<Tag>? tags = null,
            string? uniqueId = null
        ) : base(
            name: name,
            entityType: typeof(Server),
            description: description,
            properties: properties,
            propertyDefinitions: propertyDefinitions,
            tags: tags,
            uniqueId: uniqueId
        )
        {
        }

        /// <summary>
        /// Convert the Server object to a Dictionary for serialization
        /// </summary>
        /// <returns>The Dictionary representation of the object</returns>
        internal Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new()
            {
                ["name"] = Name,
                ["unique_id"] = UniqueId
            };

            if (!string.IsNullOrEmpty(Description)) { payload.Add("description", Description); }
            if (Properties.Any()) { payload.Add("custom_properties", Properties); }
            if (Tags.Any()) { payload.Add("tags", (from t in Tags select t.ToDictionary()).ToList()); }

            return payload;
        }
    }
}
