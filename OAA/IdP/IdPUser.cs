using System.Diagnostics;
using Veza.OAA.Base;

namespace Veza.OAA.IdP
{
    public class IdPUser : VezaEntity
    {
        public List<string> AssumedRoles { get; set; }
        public string? Department { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public Dictionary<string, IdPGroup> Groups { get; set; }
        public string? Identity { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsGuest { get; set; }
        public string? ManagerId { get; set; }
        public Dictionary<string, string>? SourceIdentity { get; set; }

        public IdPUser(
            string name,
            string? email = null,
            string? fullName = null,
            string? identity = null,
            PropertyDefinitions? propertyDefinitions = null) :
            base(
                name: name,
                entityType: typeof(IdPUser),
                propertyDefinitions: propertyDefinitions
            )
        {
            AssumedRoles = [];
            Email = email;
            FullName = fullName;
            Groups = [];
            Identity = identity;
        }

        /// <summary>
        /// Add an AWS IAM Role ARN that can be assumed by the user
        /// </summary>
        /// <param name="assumedRole">The assumed role ARN</param>
        /// <returns>
        /// The updated list of assumed roles
        /// </returns>
        public List<string> AddAssumedRole(string assumedRole)
        {
            if (AssumedRoles.Contains(assumedRole))
            {
                Trace.TraceInformation($"Assumed role {assumedRole} already assigned to user {Name}");
            }
            else
            {
                AssumedRoles.Add(assumedRole);
            }
            return AssumedRoles;
        }

        /// <summary>
        /// Add a list of AWS IAM Role ARNs that can be assumed by the user
        /// </summary>
        /// <param name="assumedRoles">The assumed role ARNs</param>
        /// <returns>
        /// The updated dictionary of assumed roles
        /// </returns>
        public List<string> AddAssumedRoles(List<string> assumedRoles)
        {
            foreach (string assumedRole in assumedRoles)
            {
                AddAssumedRole(assumedRole);
            }

            return AssumedRoles;
        }

        /// <summary>
        /// Add a group to the IdPUser
        /// </summary>
        /// <param name="group">The group to add</param>
        /// <returns>
        /// The updated list of groups
        /// </returns>
        public Dictionary<string, IdPGroup> AddGroup(IdPGroup group)
        {
            string identity = group.Name;
            if (group.Identity != null)
            {
                identity = group.Identity;
            }

            if (Groups.ContainsKey(identity))
            {
                throw new ArgumentException($"User already added to group {identity}");
            }
            Groups.Add(identity, group);
            return Groups;
        }

        /// <summary>
        /// Add a list of groups to the IdPUser
        /// </summary>
        /// <param name="groups">The groups to add</param>
        /// <returns>
        /// The updated dictionary of groups
        /// </returns>
        public Dictionary<string, IdPGroup> AddGroups(List<IdPGroup> groups)
        {
            foreach (IdPGroup group in groups)
            {
                string identity = group.Name;
                if (group.Identity != null)
                {
                    identity = group.Identity;
                }

                if (!Groups.TryAdd(identity, group))
                {
                    throw new ArgumentException($"User already added to group {identity}");
                }
            }
            return Groups;
        }

        /// <summary>
        /// Set the source identity for the IdPUser
        /// </summary>
        /// <param name="sourceIdentity">The source identity</param>
        /// <param name="idpProvider">The IdPProviderType</param>
        /// <returns>
        /// The updated source identity
        /// </returns>
        public Dictionary<string, string> SetSourceIdentity(string sourceIdentity, IdPProviderType idpProvider)
        {
            SourceIdentity = new Dictionary<string, string>()
            {
                { "identity", sourceIdentity },
                { "provider_type", idpProvider.ToString() }
            };
            return SourceIdentity;
        }

        /// <summary>
        /// Return a serializable dictionary representation of the IdPUser
        /// </summary>
        /// <returns>
        /// Dictionary representation of the IdPUser
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            List<Dictionary<string, string>>? assumedRoles = null;
            if (AssumedRoles.Count != 0) { 
                assumedRoles = [];
                foreach (string assumedRole in AssumedRoles)
                {
                    assumedRoles.Add(new Dictionary<string, string>() { { "identity", assumedRole } });
                } 
            }

            List<Dictionary<string, string>> groups = [];
            foreach (IdPGroup group in Groups.Values)
            {
                groups.Add(new Dictionary<string, string>() { { "identity", group.Identity } });
            }

            Dictionary<string, object> payload = new()
            {
                { "assumed_role_arns", assumedRoles },
                { "department", Department},
                { "email", Email },
                { "full_name", FullName },
                { "groups", groups },
                { "is_active", IsActive },
                { "is_guest", IsGuest },
                { "manager_id", ManagerId },
                { "name", Name},
                { "source_identity", SourceIdentity }
            };

            if (Identity != null) 
            { 
                payload.Add("identity", Identity); 
            }
            else 
            { 
                payload.Add("identity", Email);
            
            }

            if (Properties.Count != 0) { payload.Add("custom_properties", Properties); }
            if (Tags.Count != 0) { payload.Add("tags", Tags); }

            payload = (from kv in payload
                       where kv.Value != null 
                       select kv).ToDictionary(kv => kv.Key, kv => kv.Value);

            return payload;
        }
        
        /// <summary>
        /// Return a string representation of the IdPUser
        /// </summary>
        /// <returns>
        /// A string representation of the IdPUser
        /// </returns>
        public override string ToString()
        {
            return $"IdP User - {Name} ({Identity})";
        }
    }
}