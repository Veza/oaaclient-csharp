using Veza.OAA;
using Veza.OAA.IdP;
using Veza.OAA.Base;
using Veza.OAA.Exceptions;

namespace Veza.OAATest.IdPTest
{
    [TestClass]
    public class DomainTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            IdPDomain domain = new (name: "example.local");
            Assert.AreEqual(domain.ToString(), "IdP Domain - example.local");
        }
    }

    [TestClass]
    public class GroupTest
    {
        [TestMethod]
        public void TestPartialConstructor()
        {
            IdPGroup group = new (name: "Test Group");
            Assert.AreEqual(group.ToString(), "IdP Group - Test Group (Test Group)");
            Assert.IsNull(group.FullName);
        }

        [TestMethod]
        public void TestFullConstructor()
        {
            IdPGroup group = new (
                name: "Test Group",
                fullName: "Test Group Full Name",
                identity: "testgroup@example.local"
            );

            Assert.AreEqual(group.ToString(), "IdP Group - Test Group (testgroup@example.local)");
            Assert.AreEqual(group.FullName, "Test Group Full Name");
            Assert.AreEqual(group.Identity, "testgroup@example.local");
        }

        [TestMethod]
        public void TestAddAssumedRole()
        {
            IdPGroup group = new (name: "Test Group");
            group.AddAssumedRole("arn:aws:iam::123456789012:role/RoleName");
            Assert.IsTrue(group.AssumedRoles.Contains("arn:aws:iam::123456789012:role/RoleName"));
            Assert.AreEqual(group.AssumedRoles.Count, 1);
        }

        [TestMethod]
        public void TestAddDuplicateAssumedRole()
        {
            IdPGroup group = new (name: "Test Group");
            group.AddAssumedRole("arn:aws:iam::123456789012:role/RoleName");
            group.AddAssumedRole("arn:aws:iam::123456789012:role/RoleName");
            Assert.IsTrue(group.AssumedRoles.Contains("arn:aws:iam::123456789012:role/RoleName"));
            Assert.AreEqual(group.AssumedRoles.Count, 1);
        }

        [TestMethod]
        public void TestAddMultipleAssumedRoles()
        {
            List<string> assumedRoles =
            [
                "arn:aws:iam::123456789012:role/RoleName",
                "arn:aws:iam::123456789012:role/RoleName2"
            ];

            IdPGroup group = new (name: "Test Group");
            group.AddAssumedRoles(assumedRoles);
            Assert.IsTrue(group.AssumedRoles.Contains("arn:aws:iam::123456789012:role/RoleName"));
            Assert.IsTrue(group.AssumedRoles.Contains("arn:aws:iam::123456789012:role/RoleName2"));
            Assert.AreEqual(group.AssumedRoles.Count, 2);
        }

        [TestMethod]
        public void TestAddGroup()
        {
            IdPGroup group = new (name: "Test Group");
            IdPGroup subgroup = new (name: "Test Subgroup");
            group.AddGroup(subgroup);
            Assert.IsTrue(group.Groups.Contains(subgroup));
            Assert.AreEqual(group.Groups.Count, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(TemplateException), "Cannot add group to itself as a subgroup")]
        public void TestAddSelfGroup()
        {
            IdPGroup group = new (name: "Test Group");
            group.AddGroup(group);
        }

        [TestMethod]
        public void TestAddMultipleGroups()
        {
            List<IdPGroup> groups =
            [
                new IdPGroup(name: "Test Group 2"),
                new IdPGroup(name: "Test Group 3")
            ];

            IdPGroup group = new (name: "Test Group");
            group.AddGroups(groups);
            Assert.IsTrue(group.Groups.Contains(groups[0]));
            Assert.IsTrue(group.Groups.Contains(groups[1]));
            Assert.AreEqual(group.Groups.Count, 2);
        }
    }

    [TestClass]
    public class ProviderTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            IdPProvider provider = new (
                name: "Test Provider",
                type: "LDAP",
                domain: "ldaps://example.com"
            );

            Assert.IsTrue(provider.Users.Count == 0);
            Assert.IsTrue(provider.Groups.Count == 0);
            Assert.AreEqual(provider.ToString(), "IdP Provider Test Provider - LDAP");
        }

        [TestMethod]
        public void TestAddGroup()
        {
            IdPProvider provider = new (
                name: "Test Provider",
                type: "LDAP",
                domain: "ldaps://example.com"
            );

            IdPGroup group = new (
                name: "Test Group",
                fullName: "Test Group Full Name",
                identity: "testgroup@example.local"
            );

            provider.AddGroup(group);
            Assert.IsTrue(provider.Groups.ContainsKey("testgroup@example.local"));
            Assert.AreEqual(provider.Groups.Count, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Group with identifier testgroup@example.local already exists")]
        public void TestAddDuplicateGroup()
        {
            IdPProvider provider = new (
                name: "Test Provider",
                type: "LDAP",
                domain: "ldaps://example.com"
            );

            IdPGroup group = new (
                name: "Test Group",
                fullName: "Test Group Full Name",
                identity: "testgroup@example.local"
            );

            provider.AddGroup(group);
            provider.AddGroup(group);
        }

        [TestMethod]
        public void TestAddUser()
        {
            IdPProvider provider = new (
                name: "Test Provider",
                type: "LDAP",
                domain: "ldaps://example.com"
            );

            IdPUser user = new (
                name: "John Doe",
                email: "jdoe@example.com",
                identity: "jdoe@example.local"
            );

            provider.AddUser(user);
            Assert.IsTrue(provider.Users.ContainsKey("jdoe@example.local"));
            Assert.AreEqual(provider.Users.Count, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "User with identifier jdoe@example.com")]
        public void TestAddDuplicateUser()
        {
            IdPProvider provider = new (
                name: "Test Provider",
                type: "LDAP",
                domain: "ldaps://example.com"
            );

            IdPUser user = new (
                name: "John Doe",
                email: "jdoe@example.com"
            );

            provider.AddUser(user);
            provider.AddUser(user);
        }
    }

    [TestClass]
    public class UserTest
    {
        [TestMethod]
        public void TestPartialConstructor()
        {
            IdPUser user = new (name: "John Doe");
            Assert.AreEqual(user.ToString(), "IdP User - John Doe ()");
            Assert.IsNull(user.Email);
        }

        [TestMethod]
        public void TestFullConstructor()
        {
            IdPUser user = new (
                name: "John Doe",
                email: "jdoe@example.com",
                identity: "jdoe@example.local",
                fullName: "Jonathon Franklin Doe"
            );

            Assert.AreEqual(user.ToString(), "IdP User - John Doe (jdoe@example.local)");
            Assert.AreEqual(user.Email, "jdoe@example.com");
            Assert.AreEqual(user.Identity, "jdoe@example.local");
        }

        [TestMethod]
        public void TestAddAssumedRole()
        {
            IdPUser user = new (name: "John Doe");
            user.AddAssumedRole("arn:aws:iam::123456789012:role/RoleName");
            Assert.IsTrue(user.AssumedRoles.Contains("arn:aws:iam::123456789012:role/RoleName"));
            Assert.AreEqual(user.AssumedRoles.Count, 1);
        }

        [TestMethod]
        public void TestAddDuplicateAssumedRole()
        {
            IdPUser user = new (name: "John Doe");
            user.AddAssumedRole("arn:aws:iam::123456789012:role/RoleName");
            user.AddAssumedRole("arn:aws:iam::123456789012:role/RoleName");
            Assert.IsTrue(user.AssumedRoles.Contains("arn:aws:iam::123456789012:role/RoleName"));
            Assert.AreEqual(user.AssumedRoles.Count, 1);
        }

        [TestMethod]
        public void TestAddMultipleAssumedRoles()
        {
            List<string> assumedRoles =
            [
                "arn:aws:iam::123456789012:role/RoleName",
                "arn:aws:iam::123456789012:role/RoleName2"
            ];

            IdPUser user = new (name: "John Doe");
            user.AddAssumedRoles(assumedRoles);
            Assert.IsTrue(user.AssumedRoles.Contains("arn:aws:iam::123456789012:role/RoleName"));
            Assert.IsTrue(user.AssumedRoles.Contains("arn:aws:iam::123456789012:role/RoleName2"));
            Assert.AreEqual(user.AssumedRoles.Count, 2);
        }

        [TestMethod]
        public void TestAddGroup()
        {
            IdPUser user = new (name: "John Doe");
            IdPGroup group = new (name: "Test Group");
            user.AddGroup(group);
            Assert.IsTrue(user.Groups.ContainsKey("Test Group"));
            Assert.AreEqual(user.Groups.Count, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "User already added to group Test Group")]
        public void TestAddDuplicateGroup()
        {
            IdPUser user = new (name: "John Doe");
            IdPGroup group = new (name: "Test Group");
            user.AddGroup(group);
            user.AddGroup(group);
        }

        [TestMethod]
        public void TestAddMultipleGroups()
        {
            List<IdPGroup> groups =
            [
                new IdPGroup(name: "Test Group"),
                new IdPGroup(name: "Test Group 2")
            ];

            IdPUser user = new (name: "John Doe");
            user.AddGroups(groups);
            Assert.IsTrue(user.Groups.ContainsKey("Test Group"));
            Assert.IsTrue(user.Groups.ContainsKey("Test Group 2"));
            Assert.AreEqual(user.Groups.Count, 2);
        }

        [TestMethod]
        public void TestSetSourceIdentity()
        {
            IdPUser user = new (name: "John Doe");
            user.SetSourceIdentity("jeff@example.onmicrosoft.com", IdPProviderType.azure_ad);
        }
    }
}