using Veza.OAA.Base;

namespace Veza.OAA.IdP
{
    public class IdPDomain : VezaEntity
    {
        public IdPDomain(
            string name,
            PropertyDefinitions? propertyDefinitions = null) :
            base(
                name: name,
                entityType: typeof(IdPDomain),
                propertyDefinitions: propertyDefinitions
            )
        {  }
        
        /// <summary>
        /// Return a serializable dictionary representation of the IdPDomain
        /// </summary>
        /// <returns>
        /// Dictionary representation of the IdPDomain
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new()
            {
                { "name", Name }
            };
            if (Properties.Count != 0 ) { payload.Add("custom_properties", Properties); }
            if (Tags.Count != 0) { payload.Add("tags", Tags); }

            return payload;
        }

        /// <summary>
        /// Return a string representation of the IdPDomain
        /// </summary>
        /// <returns>
        /// A string representation of the IdPDomain
        /// </returns>
        public override string ToString()
        {
            return $"IdP Domain - {Name}";
        }
    }
}