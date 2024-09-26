using Veza.OAA.Base;

namespace Veza.OAA.HRIS
{
    public class HRISSystem : VezaEntity
    {
        public string Url { get; set; }
        public List<IdPProviderType> IdPProviders { get; set; }

        public HRISSystem(
            string name,
            string url = "") : 
            base(
                name: name,
                entityType: typeof(HRISSystem),
                uniqueId: name
            )
        {
            Url = url;
            IdPProviders = [];
        }

    ///<summary>
    /// Add an IdPProvider to the HRIS HRISSystem
    /// Sets the IdP type(s) (Okta, AzureAD, etc) that Veza will use to 
    /// link employee identities
    ///</summary>
    ///<param name="idpProviderType">The IdPProviderType to add</param>
    ///<returns>
    /// The updated list of IdPProviders
    /// </returns>
    public List<IdPProviderType> AddIdPProvider(IdPProviderType idpProviderType)
    {
        if (!IdPProviders.Contains(idpProviderType))
        {
            IdPProviders.Add(idpProviderType);
        }
    
        return IdPProviders;
    }

    ///<summary>
    /// Return a serializable dictionary representation of the HRISSystem
    ///</summary>
    ///<returns>
    /// Dictionary representation of the HRISSystem
    /// </returns>
    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> payload = new()
        {
            { "id", UniqueId},
            { "name", Name },
            { "url", Url },
            { "idp_providers", (from i in IdPProviders select i.ToString()) }
        };

        return payload;
    }

    /// <summary>
    /// Return a string representation of the HRISSystem
    /// </summary>
    /// <returns>
    /// String representation of the HRISSystem
    /// </returns>
    public override string ToString()
    {
        return $"HRIS System - {Name}";
    }
    }
}