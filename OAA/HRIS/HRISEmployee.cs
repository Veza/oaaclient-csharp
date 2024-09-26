using Veza.OAA.Base;

namespace Veza.OAA.HRIS
{
    public class HRISEmployee(
        string uniqueId,
        string name,
        string employeeNumber,
        string employmentStatus,
        string firstName,
        string lastName,
        bool isActive = true,
        PropertyDefinitions? propertyDefinitions = null) : VezaEntity(
            name: name,
            entityType: typeof(HRISEmployee),
            propertyDefinitions: propertyDefinitions,
            uniqueId: uniqueId)
    {
        public string? CanonicalName { get; set; }
        public string? Company { get; set; }
        public HRISGroup? CostCenter { get; set; }
        public HRISGroup? Department { get; set; }
        public string? DisplayName { get; set; }
        public string EmployeeNumber { get; set; } = employeeNumber;
        public string EmploymentStatus { get; set; } = employmentStatus;
        public List<string>? EmploymentTypes { get; set; }
        public string? Email { get; set; }
        public string FirstName { get; set; } = firstName;
        public Dictionary<string, HRISGroup> Groups { get; set; } = [];
        public string? HomeLocation { get; set; }
        public string? IdPId { get; set; }
        public bool IsActive { get; set; } = isActive;
        public string? JobTitle { get; set; }
        public string LastName { get; set; } = lastName;
        public Dictionary<string, HRISEmployee> Managers { get; set; } = [];
        public string? PersonalEmail { get; set; }
        public string? PreferredName { get; set; }
        public string? PrimaryTimeZone { get; set; }
        public string? StartDate { get; set; }
        public string? TerminationDate { get; set; }
        public string? UserName { get; set; }
        public string? WorkLocation { get; set; }

        /// <summary>
        /// Add an HRISEmployee to a group
        /// </summary>
        /// <param name="group">The group to which to add the HRISEmployee</param>
        public void AddGroup(HRISGroup group)
        {
            Groups.TryAdd(group.UniqueId, group);
        }

        /// <summary>
        /// Add a manager to the HRISEmployee
        /// </summary>
        /// <param name="manager">The manager to add</param>
        public void AddManager(HRISEmployee manager)
        {
            Managers.TryAdd(manager.UniqueId, manager);
        }

        /// <summary>
        /// Return a serializable dictionary representation of the HRIS HRISEmployee
        /// </summary>
        /// <returns>
        /// Dictionary representation of the HRIS HRISEmployee
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payload = new()
            {
                { "company", Company},
                { "display_name", DisplayName},
                { "canonical_name", CanonicalName},
                { "email", Email},
                { "HRISEmployee_number", EmployeeNumber },
                { "employment_status", EmploymentStatus },
                { "employment_types", EmploymentTypes},
                { "first_name", FirstName },
                { "home_location", HomeLocation},
                { "idp_id", IdPId},
                { "id", UniqueId },
                { "is_active", IsActive },
                { "job_title", JobTitle},
                { "last_name", LastName },
                { "name", Name},
                { "personal_email", PersonalEmail},
                { "preferred_name", PreferredName},
                { "primary_time_zone", PrimaryTimeZone},
                { "user_name", UserName},
                { "work_location", WorkLocation},
            };

            if (CostCenter != null) { payload.Add("cost_center", new Dictionary<string, string>() { { "id", CostCenter.UniqueId } }); }
            if (Department != null) { payload.Add("department", new Dictionary<string, string>() { { "id", Department.UniqueId } }); }
            
            List<Dictionary<string, string>> groups = [];
            foreach (string groupId in Groups.Keys)
            {
                groups.Add(new Dictionary<string, string>() { { "id", groupId } });
            };
            if (groups.Count != 0) { payload.Add("groups", groups); }

            List<Dictionary<string, string>> managers = [];
            foreach (string managerId in Managers.Keys)
            {
                managers.Add(new Dictionary<string, string>() { { "id", managerId } });
            }
            if (managers.Count != 0) { payload.Add("managers", managers); }

            if (StartDate != null) { payload.Add("start_date", StartDate); }
            if (TerminationDate != null) { payload.Add("termination_date", TerminationDate); }
            if (Properties.Count != 0) { payload.Add("custom_properties", Properties); }

            payload = (from kv in payload
                       where kv.Value != null 
                       select kv).ToDictionary(kv => kv.Key, kv => kv.Value);

            return payload;
        }

        /// <summary>
        /// Return a string representation of the HRIS HRISEmployee
        /// </summary>
        /// <returns>
        /// String representation of the HRIS HRISEmployee
        /// </returns>
        public override string ToString()
        {
            return $"HRIS Employee - {Name} ({UniqueId})";
        }
    }
}