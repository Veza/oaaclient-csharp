using System.Diagnostics;
using Veza.OAA.Base;
using Veza.OAA.Exceptions;

namespace Veza.OAA.IdP
{
    public class IdPGroup : VezaEntity
    {
        public List<string> AssumedRoles { get; set; }
        public string? FullName { get; set; }
        public List<IdPGroup> Groups { get; set; }
        public string Identity { get; set; }
        public bool IsSecurityGroup { get; set; }

        public IdPGroup(
            string name,
            string? fullName = null,
            string identity = null,
            PropertyDefinitions? propertyDefinitions = null) :
            base(
                name: name,
                entityType: typeof(IdPGroup),
                propertyDefinitions: propertyDefinitions
            )
        {
            FullName = fullName;
            Identity = identity ?? name;
            AssumedRoles = [];
            Groups = [];
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
        /// The updated list of assumed roles
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
        /// <exception cref="TemplateException">
        /// Thrown when the group is the same as the IdPGroup
        /// </exception>
        public List<IdPGroup> AddGroup(IdPGroup group)
        {
            if (Groups.Contains(group)) 
            { 
                Trace.TraceInformation($"Group {group.Name} already assigned to {Name} as a subgroup");
            }
            else if (group == this) 
            { 
                throw new TemplateException("Cannot add group to itself as a subgroup");
            }

            Groups.Add(group);
            return Groups;
        }

        /// <summary>
        /// Add a list of groups to the IdPUser
        /// </summary>
        /// <param name="groups">The groups to add</param>
        /// <returns>
        /// The updated list of groups
        /// </returns>
        /// <exception cref="TemplateException">
        /// Thrown when a group in the list is the same as the IdPGroup
        /// </exception>
        public List<IdPGroup> AddGroups(List<IdPGroup> groups)
        {
            foreach (IdPGroup group in groups)
            {
                AddGroup(group);
            }
            return Groups;
        }

        /// <summary>
        /// Return a serializable dictionary representation of the IdPGroup
        /// </summary>
        /// <returns>
        /// Dictionary representation of the IdPGroup
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            List<Dictionary<string, string>>? assumedRoles = null;
            if (AssumedRoles.Count != 0 ) { 
                assumedRoles = [];
                foreach (string assumedRole in AssumedRoles)
                {
                    assumedRoles.Add(new Dictionary<string, string>() { { "identity", assumedRole } });
                } 
            }


            List<Dictionary<string, string>> groups = [];
            foreach (IdPGroup group in Groups)
            {
                groups.Add(new Dictionary<string, string>() { { "identity", group.Identity } });
            }

            Dictionary<string, object> payload = new()
            {
                { "assumed_role_arns", assumedRoles },
                { "full_name", FullName },
                { "groups", groups },
                { "is_security_group", IsSecurityGroup },
                { "name", Name },
                { "identity", Identity ?? Name }
            };

            if (Properties.Count != 0) { payload.Add("custom_properties", Properties); }
            if (Tags.Count != 0) { payload.Add("tags", Tags); }

            return payload;
        }

        /// <summary>
        /// Return a string representation of the IdPGroup
        /// </summary>
        /// <returns>
        /// A string representation of the IdPGroup
        /// </returns>
        public override string ToString()
        {
            return $"IdP Group - {Name} ({Identity})";
        }
    }
}