using Veza.OAA.Base;

namespace Veza.OAA.Application
{
    /// <summary>
    /// The base class for a CustomApplication
    /// </summary>
    public abstract class Application : VezaEntity
    {
        internal string ApplicationType { get; set; }

        internal Application(
            string applicationType,
            string name,
            string? description = null) : 
            base (
                name: name, 
                description: description,
                entityType: typeof(Application)
            )
        {
            ApplicationType = applicationType;
        }
    }

}
