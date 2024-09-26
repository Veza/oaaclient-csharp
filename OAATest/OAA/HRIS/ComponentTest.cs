using Veza.OAA;
using Veza.OAA.HRIS;

namespace Veza.OAATest.HRISTest
{
    [TestClass]
    public class EmployeeTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            HRISEmployee employee = new HRISEmployee(
                uniqueId: "1234",
                name: "John Doe",
                employeeNumber: "1234",
                firstName: "John",
                lastName: "Doe",
                isActive: true,
                employmentStatus: "FULL_TIME"
            );

            Assert.AreEqual(employee.ToString(), "HRIS Employee - John Doe (1234)");
            Assert.IsNull(employee.CanonicalName);
        }

        [TestMethod]
        public void TestAddGroup()
        {
            HRISEmployee employee = new HRISEmployee(
                uniqueId: "1234",
                name: "John Doe",
                employeeNumber: "1234",
                firstName: "John",
                lastName: "Doe",
                isActive: true,
                employmentStatus: "FULL_TIME"
            );

            HRISGroup group = new HRISGroup(
                name: "Test Group",
                groupType: "Test",
                uniqueId: "1234"
            );

            employee.AddGroup(group);

            Assert.IsTrue(employee.Groups.ContainsKey(group.UniqueId));
        }

        [TestMethod]
        public void TestAddGroupById()
        {
            HRISEmployee employee = new HRISEmployee(
                uniqueId: "1234",
                name: "John Doe",
                employeeNumber: "1234",
                firstName: "John",
                lastName: "Doe",
                isActive: true,
                employmentStatus: "FULL_TIME"
            );

            HRISGroup group = new HRISGroup(
                name: "Test Group",
                groupType: "Test",
                uniqueId: "1234"
            );

            employee.AddGroup(group);

            Assert.IsTrue(employee.Groups.ContainsKey(group.UniqueId));
        }

        [TestMethod]
        public void TestAddManager()
        {
            HRISEmployee employee = new HRISEmployee(
                uniqueId: "1234",
                name: "John Doe",
                employeeNumber: "1234",
                firstName: "John",
                lastName: "Doe",
                isActive: true,
                employmentStatus: "FULL_TIME"
            );

            HRISEmployee manager = new HRISEmployee(
                uniqueId: "5678",
                name: "Jane Doe",
                employeeNumber: "5678",
                firstName: "Jane",
                lastName: "Doe",
                isActive: true,
                employmentStatus: "FULL_TIME"
            );

            employee.AddManager(manager);

            Assert.IsTrue(employee.Managers.ContainsKey(manager.UniqueId));
        }

        [TestMethod]
        public void TestAddManagerById()
        {
            HRISEmployee employee = new HRISEmployee(
                uniqueId: "1234",
                name: "John Doe",
                employeeNumber: "1234",
                firstName: "John",
                lastName: "Doe",
                isActive: true,
                employmentStatus: "FULL_TIME"
            );

            HRISEmployee manager = new HRISEmployee(
                uniqueId: "5678",
                name: "Jane Doe",
                employeeNumber: "5678",
                firstName: "Jane",
                lastName: "Doe",
                isActive: true,
                employmentStatus: "FULL_TIME"
            );

            employee.AddManager(manager);

            Assert.IsTrue(employee.Managers.ContainsKey(manager.UniqueId));
        }
    }

    [TestClass]
    public class GroupTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            HRISGroup group = new HRISGroup(
                name: "Test Group",
                groupType: "Test",
                uniqueId: "1234"
            );

            Assert.AreEqual(group.ToString(), "HRIS Group - Test Group (1234) - Test");
        }
    }

    [TestClass]
    public class ProviderTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            HRISProvider provider = new HRISProvider(
                name: "Test Provider",
                hrisType: "Test",
                url: "https://example.com"
            );

            Assert.IsTrue(provider.Employees.Count == 0);
            Assert.IsTrue(provider.Groups.Count == 0);
            Assert.AreEqual(provider.ToString(), "HRIS Provider - Test Provider - Test");
        }

        [TestMethod]
        public void TestAddEmployee()
        {
            HRISProvider provider = new HRISProvider(
                name: "Test Provider",
                hrisType: "Test",
                url: "https://example.com"
            );

            HRISEmployee employee = provider.AddEmployee(
                uniqueId: "1234",
                name: "John Doe",
                employeeNumber: "1234",
                firstName: "John",
                lastName: "Doe",
                isActive: true,
                employmentStatus: "FULL_TIME"
            );

            Assert.IsTrue(provider.Employees.ContainsKey(employee.UniqueId));
            Assert.IsTrue(provider.Employees.Count == 1);
        }

        [TestMethod]
        public void TestAddGroup()
        {
            HRISProvider provider = new HRISProvider(
                name: "Test Provider",
                hrisType: "Test",
                url: "https://example.com"
            );

            HRISGroup group = new HRISGroup(
                name: "Test Group",
                groupType: "Test",
                uniqueId: "1234"
            );

            provider.AddGroup(group);

            Assert.IsTrue(provider.Groups.ContainsKey(group.UniqueId));
            Assert.IsTrue(provider.Groups.Count == 1);
        }
    }

    [TestClass]
    public class SystemTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            HRISSystem system = new HRISSystem(
                name: "Test System",
                url: "https://example.com"
            );

            Assert.AreEqual(system.ToString(), "HRIS System - Test System");
        }

        [TestMethod]
        public void TestAddIdPProvider()
        {
            HRISSystem system = new HRISSystem(
                name: "Test System",
                url: "https://example.com"
            );

            system.AddIdPProvider(IdPProviderType.okta);

            Assert.AreEqual(system.IdPProviders.Count, 1);
        }
    }
}