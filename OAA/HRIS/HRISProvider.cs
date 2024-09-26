using System.Text.Json;
using Veza.OAA.Base;

namespace Veza.OAA.HRIS
{
    public class HRISProvider : VezaEntity
    {
        public Dictionary<Type, PropertyDefinitions> DefinedProperties { get; set; }
        public Dictionary<string, HRISEmployee> Employees { get; set; }
        public Dictionary<string, HRISGroup> Groups { get; set; }
        public HRISSystem HRISSystem { get; set; }
        public string HRISType { get; set; }

        public HRISProvider(
            string name,
            string hrisType,
            string url) :
            base(
                name: name,
                entityType: typeof(HRISProvider)
            )
        {
            HRISType = hrisType;
            HRISSystem = new HRISSystem(name: name, url: url);

            DefinedProperties = new Dictionary<Type, PropertyDefinitions>()
                {
                    { typeof(HRISEmployee), new PropertyDefinitions() },
                    { typeof(HRISGroup), new PropertyDefinitions() },
                    { typeof(HRISSystem), new PropertyDefinitions() }
                };
            Employees = new Dictionary<string, HRISEmployee>(StringComparer.OrdinalIgnoreCase);
            Groups = new Dictionary<string, HRISGroup>(StringComparer.OrdinalIgnoreCase);
        }

    /// <summary>
    /// Add an Employee to the HRIS Provider
    /// </summary>
    /// <param name="employee">The employee to add</param>
    /// <returns>
    /// The added Employee
    /// </returns>
    public HRISEmployee AddEmployee(HRISEmployee employee)
    {
        if (Employees.ContainsKey(employee.UniqueId))
        {
            throw new ArgumentException($"Employee with unique_id {employee.UniqueId} already exists");
        }

        if (employee.PropertyDefinitions is null)
        {
            employee.PropertyDefinitions = DefinedProperties[typeof(HRISEmployee)];
        }

        Employees.Add(employee.UniqueId, employee);
        return employee;
    }

    /// <summary>
    /// Add an Employee to the HRIS Provider
    /// </summary>
    /// <param name="uniqueId">The unique identifier for the employee</param>
    /// <param name="name">The name of the employee</param>
    /// <param name="employeeNumber">The employee number</param>
    /// <param name="firstName">The first name of the employee</param>
    /// <param name="lastName">The last name of the employee</param>
    /// <param name="isActive">Whether the employee is active</param>
    /// <param name="employmentStatus">The employment status of the employee</param>
    /// <returns>
    /// The added Employee
    /// </returns>
    public HRISEmployee AddEmployee(string uniqueId, string name, string employeeNumber, string firstName, 
                                string lastName, bool isActive, string employmentStatus)
    {
        if (Employees.ContainsKey(uniqueId))
        {
            throw new ArgumentException($"Employee with unique_id {uniqueId} already exists");
        }

        HRISEmployee employee = new HRISEmployee(
            uniqueId: uniqueId,
            name: name,
            employeeNumber: employeeNumber,
            firstName: firstName,
            lastName: lastName,
            isActive: isActive,
            employmentStatus: employmentStatus,
            propertyDefinitions: DefinedProperties[typeof(HRISEmployee)]
        );

        Employees.Add(uniqueId, employee);
        return employee;
    }

    /// <summary>
    /// Add a group to the HRIS Provider
    /// </summary>
    /// <param name="group">The group to add</param>
    /// <returns>
    /// The added Group
    /// </returns>
    public HRISGroup AddGroup(HRISGroup group)
    {
        if (Groups.ContainsKey(group.UniqueId))
        {
            throw new ArgumentException($"Group with ID {group.UniqueId} already exists in the Provider");
        }

        if (group.PropertyDefinitions is null)
        {
            group.PropertyDefinitions = DefinedProperties[typeof(HRISGroup)];
        }

        Groups.Add(group.UniqueId, group);
        return group;
    }

    /// <summary>
    /// Add a Group to the HRIS Provider
    /// </summary>
    /// <param name="uniqueId">The unique identifier for the group</param>
    /// <param name="name">The name of the group</param>
    /// <param name="groupType">The type of the group</param>
    /// <returns>
    /// The added Group
    /// </returns>
    public HRISGroup AddGroup(string uniqueId, string name, string groupType)
    {
        if (Groups.ContainsKey(uniqueId))
        {
            throw new ArgumentException($"Group with ID {uniqueId} already exists");
        }

        HRISGroup group = new HRISGroup(
            uniqueId: uniqueId,
            name: name,
            groupType: groupType,
            propertyDefinitions: DefinedProperties[typeof(HRISGroup)]
        );
        Groups.Add(uniqueId, group);
        return group;
    }

    /// <summary>
    /// Return a serializable dictionary representation of the HRISProvider
    /// </summary>
    /// <returns>
    /// Dictionary representation of the HRISProvider
    /// </returns>
    public Dictionary<string, object> GetPayload()
    {
        return ToDictionary();
    }

    /// <summary>
    /// Get the Veza payload JSON
    /// </summary>
    /// <returns>
    /// JSON representation of the HRISProvider
    /// </returns>
    public string GetJSONPayload()
    {
        JsonSerializerOptions json_options = new() { WriteIndented = true };
        return JsonSerializer.Serialize(GetPayload(), json_options);
    }

    /// <summary>
    /// Return a serializable dictionary representation of the HRISProvider
    /// </summary>
    /// <returns>
    /// Dictionary representation of the HRISProvider
    /// </returns>
    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> propertyDefinitions = new()
        {
            { "employee_properties", DefinedProperties[typeof(HRISEmployee)].ToDictionary() },
            { "group_properties", DefinedProperties[typeof(HRISGroup)].ToDictionary() },
            { "system_properties", DefinedProperties[typeof(HRISSystem)].ToDictionary() }
        };

        Dictionary<string, object> payload = new()
        {
            { "name", Name },
            { "hris_type", HRISType },
            { "custom_property_definition", propertyDefinitions },
            { "system", HRISSystem.ToDictionary() },
            { "employees", (from e in Employees.Values select e.ToDictionary()).ToList() },
            { "groups", (from g in Groups.Values select g.ToDictionary()).ToList() }
        };

        return payload;
    }

    /// <summary>
    /// Return a string representation of the HRISProvider
    /// </summary>
    /// <returns>
    /// String representation of the HRISProvider
    /// </return>
    public override string ToString()
    {
        return $"HRIS Provider - {Name} - {HRISType}";
    }

    }
}