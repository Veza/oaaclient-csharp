using Veza.OAA;
using Veza.OAA.HRIS;

namespace Veza.OAATest.HRISTest
{
    internal static class GenerateHRIS
    {
        internal static HRISProvider GenerateHRISProvider()
        {
            HRISProvider provider = new(
                name: "MSTest HRIS",
                hrisType: "MSTestHRIS",
                url: "https://example.com"
            );

            /// add IdP providers
            provider.HRISSystem.AddIdPProvider(IdPProviderType.okta);
            provider.HRISSystem.AddIdPProvider(IdPProviderType.azure_ad);

            /// define properties
            provider.DefinedProperties[typeof(HRISEmployee)].DefineProperty("nickname", typeof(string));
            provider.DefinedProperties[typeof(HRISEmployee)].DefineProperty("has_keys", typeof(bool));
            provider.DefinedProperties[typeof(HRISGroup)].DefineProperty("is_social", typeof(bool));

            /// add employees
            for (int i = 0; i < 10; i++)
            {
                HRISEmployee employee = provider.AddEmployee(
                    uniqueId: $"employee_{i}",
                    name: $"Employee {i}",
                    employeeNumber: $"emp_{i}",
                    firstName: $"First {i}",
                    lastName: $"Last {i}",
                    isActive: true,
                    employmentStatus: "FULL_TIME"
                );

                employee.SetProperty("nickname", $"Nick {i}");
                employee.SetProperty("has_keys", i % 2 == 0);
            }

            /// add a cost center group
            HRISGroup cost_center_group = provider.AddGroup(
                uniqueId: "cc01",
                name: "Cost Center 01",
                groupType: "cost_center"
            );

            /// add a department group
            HRISGroup department_group = provider.AddGroup(
                uniqueId: "eng",
                name: "Engineering",
                groupType: "Department"
            );

            HRISEmployee employee_1 = provider.Employees["employee_1"];
            HRISEmployee employee_2 = provider.Employees["employee_2"];

            /// add fully-detailed employee
            HRISEmployee detailed_employee = provider.AddEmployee(
                uniqueId: "detailedemployee",
                name: "Detailed Employee",
                employeeNumber: "1010101",
                firstName: "Detailed",
                lastName: "Employee",
                isActive: true,
                employmentStatus: "FULL_TIME"
            );
            detailed_employee.CanonicalName = "Detailed Employee Canonical Name";
            detailed_employee.Company = "Veza";
            detailed_employee.CostCenter = cost_center_group;
            detailed_employee.Department = department_group;
            detailed_employee.DisplayName = "Detailed Employee Display Name";
            detailed_employee.Email = "detailed_empolyee@cookiestg.com";
            detailed_employee.EmploymentStatus = "Hired";
            detailed_employee.EmploymentTypes = new List<string> { "FULL_TIME" };
            detailed_employee.HomeLocation = "Des Plaines, IL";
            detailed_employee.IdPId = "detailed_employee@veza.local";
            detailed_employee.JobTitle = "Software Engineer";
            detailed_employee.Managers = new Dictionary<string, HRISEmployee>() { { "employee_1", employee_1 } };
            detailed_employee.PersonalEmail = "detailed_employee@example.com";
            detailed_employee.PreferredName = "Dee";
            detailed_employee.StartDate = "2020-04-12T23:20:50.52Z";
            detailed_employee.TerminationDate = "2023-10-01T23:20:50.52Z";
            detailed_employee.UserName = "detailed_employee";
            detailed_employee.WorkLocation = "Chicago, IL";
            detailed_employee.PrimaryTimeZone = "CST";

            detailed_employee.AddManager(employee_2);

            /// add groups
            for (int i = 0; i < 5; i++)
            {
                HRISGroup group = provider.AddGroup(
                    uniqueId: $"group_{i}",
                    name: $"Group {i}",
                    groupType: "Team"
                );
                provider.Employees[$"employee_{i}"].AddGroup(group);
            }

            provider.Groups["group_0"].SetProperty("is_social", true);
            return provider;
        }
    }
}