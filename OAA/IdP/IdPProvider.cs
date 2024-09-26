using System.Text.Json;
using System.Text.Json.Serialization;
using Veza.OAA.Base;

namespace Veza.OAA.IdP
{
    public class IdPProvider : VezaEntity
    {
        public Dictionary<Type, PropertyDefinitions> DefinedProperties { get; set; }
        public IdPDomain Domain { get; set; }
        public Dictionary<string, IdPGroup> Groups { get; set; }
        [JsonPropertyName("idp_type")]
        public string IdPType { get; set; }
        public Dictionary<string, IdPUser> Users { get; set; }

        public IdPProvider(
            string name,
            string type,
            string domain,
            string? description = null,
            PropertyDefinitions? propertyDefinitions = null) :
            base(
                name: name,
                entityType: typeof(IdPProvider),
                propertyDefinitions: propertyDefinitions
            )
        {
            DefinedProperties = new Dictionary<Type, PropertyDefinitions>()
            {
                { typeof(IdPUser), new PropertyDefinitions() },
                { typeof(IdPGroup), new PropertyDefinitions() },
                { typeof(IdPDomain), new PropertyDefinitions() }
            };
            Description = description;
            Domain = new IdPDomain(name: domain);
            Groups = new Dictionary<string, IdPGroup>(StringComparer.OrdinalIgnoreCase);
            IdPType = type;
            Users = new Dictionary<string, IdPUser>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Add an IdPGroup to the IdPProvider
        /// </summary>
        /// <param name="group">The group to add</param>
        /// <returns>
        /// The added IdPGroup
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the group already exists
        /// </exception>
        public IdPGroup AddGroup(IdPGroup group)
        {
            string identifier = group.Identity ?? group.Name;

            if (Groups.ContainsKey(identifier))
            {
                throw new ArgumentException($"Group with identifier {identifier} already exists");
            }

            Groups.Add(identifier, group);
            return group;
        }

        /// <summary>
        /// Add an IdPGroup to the IdPProvider
        /// </summary>
        /// <param name="name">The name of the group</param>
        /// <param name="fullName">The full name of the group</param>
        /// <param name="identity">The identity of the group</param>
        /// <returns>
        /// The added IdPGroup
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the group already exists
        /// </exception>
        public IdPGroup AddGroup(
            string name,
            string? fullName = null,
            string? identity = null)
        {
            string identifier = identity ?? name;
            if (Groups.ContainsKey(identifier))
            {
                throw new ArgumentException($"Group with identifier {identifier} already exists");
            }

            IdPGroup group = new(
                name: name,
                fullName: fullName,
                identity: identity,
                propertyDefinitions: DefinedProperties[typeof(IdPGroup)]
            );

            // TODO: add properties if they come in null
            return AddGroup(group);
        }

        /// <summary>
        /// Add an IdPUser to the IdPProvider
        /// </summary>
        /// <param name="user">The user to add</param>
        /// <returns>
        /// The added IdPUser
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the user already exists
        /// </exception>
        public IdPUser AddUser(IdPUser user)
        {
            string identifier = user.Identity ?? user.Name;

            if (Users.ContainsKey(identifier))
            {
                throw new ArgumentException($"User with identifier {identifier} already exists");
            }

            // TODO: add properties if they come in null
            Users.Add(identifier, user);
            return user;
        }

        /// <summary>
        /// Add an IdPUser to the IdPProvider
        /// </summary>
        /// <param name="name">The name of the user</param>
        /// <param name="email">The email of the user</param>
        /// <param name="fullName">The full name of the user</param>
        /// <param name="identity">The identity of the user</param>
        /// <returns>
        /// The added IdPUser
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the user already exists
        /// </exception>
        public IdPUser AddUser(
            string name,
            string email = null,
            string fullName = null,
            string identity = null)
        {
            string identifier = identity ?? name;

            if (Users.ContainsKey(identifier))
            {
                throw new ArgumentException($"User with identifier {identifier} already exists");
            }

            IdPUser user = new (
                name: name,
                email: email,
                fullName: fullName,
                identity: identity,
                propertyDefinitions: DefinedProperties[typeof(IdPUser)]
            );
            return AddUser(user);
        }

        /// <summary>
        /// Return a serializable dictionary representation of the IdPProvider
        /// </summary>
        /// <returns>
        /// Dictionary representation of the IdPProvider
        /// </returns>
        public Dictionary<string, object> GetPayload()
        {
            return ToDictionary();
        }

        /// <summary>
        /// Get the Veza payload JSON for the IdPProvider
        /// </summary>
        /// <returns>
        /// The Veza payload JSON for the IdPProvider
        /// </returns>
        public string GetJSONPayload()
        {
            JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
            return JsonSerializer.Serialize(GetPayload(), jsonOptions);
        }

        /// <summary>
        /// Return a serializable dictionary representation of the IdPProvider
        /// </summary>
        /// <returns>
        /// Dictionary representation of the IdPProvider
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> propertyDefinitions = new()
            {
                { "user_properties", DefinedProperties[typeof(IdPUser)].ToDictionary() },
                { "group_properties", DefinedProperties[typeof(IdPGroup)].ToDictionary() },
                { "domain_properties", DefinedProperties[typeof(IdPDomain)].ToDictionary() }
            };

            Dictionary<string, object> payload = new()
            {
                { "custom_property_definition", propertyDefinitions },
                { "name", Name },
                { "idp_type", IdPType },
                { "domains", new List<Dictionary<string, object>>() {Domain.ToDictionary() } },
                { "users", Users.Select(u => u.Value.ToDictionary()) },
                { "groups", Groups.Select(g => g.Value.ToDictionary()) }
            };
            if (Properties.Count != 0) { payload.Add("custom_properties", Properties); }
            if (Tags.Count != 0) { payload.Add("tags", Tags); }

            return payload;
        }

        /// <summary>
        /// Return a string representation of the IdPProvider
        /// </summary>
        /// <returns>
        /// A string representation of the IdPProvider
        /// </returns>
        public override string ToString()
        {
            return $"IdP Provider {Name} - {IdPType}";
        }
    }
}