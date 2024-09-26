using Veza.OAA.Base;

namespace Veza.OAA.Application
{
    /// <summary>
    /// Derived from base Identity class.
    /// 
    /// Used to represent groups of local users for the application.
    /// TODO: change identities and groups to Dict?
    /// </summary>
    public class Group : Identity
    {
        public Group(
            string name,
            List<string>? identities = null,
            List<string>? groups = null,
            string? uniqueId = null,
            PropertyDefinitions? propertyDefinitions = null) :
            base(
                groups: groups,
                identities: identities,
                identityType: IdentityType.local_group,
                name: name,
                propertyDefinitions: propertyDefinitions,
                uniqueId: uniqueId
            )
        { }

        /// <summary>
        /// Returns a dictionary representation of the Group
        /// </summary>
        /// <returns>
        /// A dictionary representation of the Group
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new() { { "name", Name } };

            if (CreatedAt != null) { payload["created_at"] = CreatedAt.ToRFC3339(); }
            if (Properties.Any()) { payload.Add("custom_properties", Properties); }
            if (Groups.Any()) { payload.Add("groups", Groups); }
            if (Identities.Any()) { payload.Add("identities", Identities); }
            if (Tags.Any()) { payload.Add("tags", Tags); }
            if (!string.IsNullOrEmpty(UniqueId)) { payload.Add("id", UniqueId); }

            return payload;
        }

        /// <summary>
        /// Return a string representation of the Group
        /// </summary>
        /// <returns>A string representation of the Group</returns>
        public override string ToString()
        {
            return $"Local Group - {Name} ({UniqueId})";
        }
    }
}
