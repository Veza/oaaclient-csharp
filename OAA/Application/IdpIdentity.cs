using Veza.OAA.Exceptions;

namespace Veza.OAA.Application
{
    /// <summary>
    /// IdP identitiy derived from the Identity base class.
    /// 
    /// Used to associate IdP identities (users or groups) directly to resources where the concept of local users/groups doesn't apply.
    /// </summary>
    public class IdPIdentity : Identity
    {

        public IdPIdentity(string name) :
            base(
                name: name,
                identityType: IdentityType.idp
            )
        { }

        /// <summary>
        /// Throw a TemplateException error if the user attempts to set a custom property on the IdPIdentity
        /// </summary>
        /// <exception cref="TemplateException"></exception>
        public static new void SetProperty(string _1, object _2)
        {
            throw new TemplateException($"IdP Identities do not support custom properties");
        }

        /// <summary>
        /// Return a string representation of the IdPIdentity object
        /// </summary>
        /// <returns>
        /// A string representation of the IdPIdentity object
        /// </returns>
        public override string ToString()
        {
            return $"IdP Identity {Name}";
        }

    }
}
