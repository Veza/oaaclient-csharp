using Veza.OAA.Base;

namespace Veza.OAA.HRIS
{
    public class HRISGroup : VezaEntity
    {
        public string GroupType { get; set; }
        public new string UniqueId { get; set; }

        public HRISGroup(
            string name,
            string groupType,
            string uniqueId,
            PropertyDefinitions? propertyDefinitions = null) :
        base(
            name: name,
            entityType: typeof(HRISGroup),
            propertyDefinitions: propertyDefinitions
        )
        {
            GroupType = groupType;
            UniqueId = uniqueId;
        }

        /// <summary>
        /// Return a serializable dictionary representation of the HRISGroup
        /// </summary>
        /// <returns>
        /// Dictionary representation of the HRISGroup
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new()
            {
                { "group_type", GroupType},
                { "id", UniqueId},
                { "name", Name }
            };
            if (Properties.Any()) { payload.Add("custom_properties", Properties); }

            return payload;
        }

        ///  <summary>
        ///  Return a string representation of the HRISGroup
        ///  </summary>
        ///  <returns>
        /// A string representation of the HRISGroup
        /// </returns>
        public override string ToString()
        {
            return $"HRIS Group - {Name} ({UniqueId}) - {GroupType}";
        }
    }
}