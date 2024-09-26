using Veza.OAA.Base;

namespace Veza.OAA.Application
{
    /// <summary>
    /// User identity, derived from the Identity Base class.
    /// 
    /// Used to model an application user.
    /// 
    /// Can be associated with an external IdP user, or represent a local account.
    /// </summary>
    public class User : Identity
    {
        public DateTime? DeactivatedAt { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? PasswordLastChangedAt { get; set; }

        public User(
            string name,
            List<string>? identities = null,
            List<string>? groups = null,
            string? uniqueId = null,
            PropertyDefinitions? propertyDefinitions = null
        ) : base(
            name: name,
            groups: groups,
            identities: identities,
            identityType: IdentityType.local_user,
            propertyDefinitions: propertyDefinitions,
            uniqueId: uniqueId
        )
        { }

        /// <summary>
        /// Return a dictionary representation of the User
        /// </summary>
        /// <returns>
        /// A dictionary representation of the Localuser
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new() { { "name", Name } };

            if (CreatedAt != null) { payload.Add("created_at", CreatedAt.ToRFC3339()); }
            if (Properties.Any()) { payload.Add("custom_properties", Properties); }
            if (DeactivatedAt != null) { payload.Add("deactivated_at", DeactivatedAt.ToRFC3339()); }
            if (Groups.Any()) { payload.Add("groups", Groups); }
            if (Identities.Any()) { payload.Add("identities", Identities); }
            if (IsActive != null) { payload.Add("is_active", IsActive); }
            if (LastLoginAt != null) { payload.Add("last_login_at", LastLoginAt.ToRFC3339()); }
            if (PasswordLastChangedAt != null) { payload.Add("password_last_changed_at", PasswordLastChangedAt.ToRFC3339()); }
            if (Tags.Any()) { payload.Add("tags", Tags); }
            if (!string.IsNullOrEmpty(UniqueId)) { payload.Add("id", UniqueId); }

            return payload;
        }

        /// <summary>
        /// Return a string representation of the User
        /// </summary>
        /// <returns>
        /// A string representation of the User
        /// </returns>
        public override string ToString()
        {
            if (UniqueId == null)
            {
                return $"Local user - {Name}";
            }
            else
            {
                return $"Local user - {Name} ({UniqueId})";
            }

        }
    }
}
