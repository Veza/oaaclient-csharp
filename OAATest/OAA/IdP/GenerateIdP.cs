using Veza.OAA;
using Veza.OAA.IdP;

namespace Veza.OAATest.IdPTest
{
    internal static class GenerateIdP
    {
        internal static IdPProvider GenerateIdPProvider()
        {
            IdPProvider idp = new(
                name: "MSTest IdP",
                type: "LDAP",
                domain: "example.com",
                description: "MSTest IdP Provider"
            );

            // define user properties
            idp.DefinedProperties[typeof(IdPUser)].DefineProperty("contractor", typeof(bool));
            idp.DefinedProperties[typeof(IdPUser)].DefineProperty("parking_space", typeof(int));
            idp.DefinedProperties[typeof(IdPUser)].DefineProperty("cube_number", typeof(string));
            idp.DefinedProperties[typeof(IdPUser)].DefineProperty("nicknames", typeof(List<string>));
            idp.DefinedProperties[typeof(IdPUser)].DefineProperty("birthday", typeof(DateTime));

            // define group properties
            idp.DefinedProperties[typeof(IdPGroup)].DefineProperty("owner", typeof(string));

            // define domain properties
            idp.DefinedProperties[typeof(IdPDomain)].DefineProperty("region", typeof(string));

            // set domain properties
            idp.Domain.PropertyDefinitions = idp.DefinedProperties[typeof(IdPDomain)];
            idp.Domain.SetProperty("region", "US");
            idp.Domain.AddTag("domain_tag");

            // add users
            IdPUser user1 = idp.AddUser(
                name: "user1",
                fullName: "User 1",
                email: "user1@example.com",
                identity: "1111"
            );
            IdPUser user2 = idp.AddUser(
                name: "user2",
                fullName: "User 2",
                email: "user2@example.com",
                identity: "2222"
            );
            IdPUser user3 = idp.AddUser(
                name: "user3",
                fullName: "User 3",
                email: "user3@example.com",
                identity: "3333"
            );
            IdPUser user4 = idp.AddUser(
                name: "user4",
                fullName: "User 4",
                email: "user4@example.com",
                identity: "4444"
            );

            // add user constructed outside of the provider
            IdPUser user5 = new IdPUser(
                name: "user5",
                fullName: "User 5",
                email: "user5@example.com",
                identity: "5555"
            );
            idp.AddUser(user5);

            user1.Department = "Quality Assurance";
            user1.IsActive = true;
            user1.IsGuest = false;
            user1.ManagerId = "3333";
            user1.SetProperty("contractor", false);
            user1.SetProperty("parking_space", 1);
            user1.SetProperty("cube_number", "A1");
            user1.SetProperty("nicknames", new List<string>() {"Nick", "One Dude"});
            user1.SetProperty("birthday", "2000-01-01T00:00:00.000Z");
            user1.SetSourceIdentity("user1@example.local", IdPProviderType.okta);
            user1.AddAssumedRole("arn:aws:iam::123456789012:role/role1");
            user1.AddTag("user1_tag");

            user2.Department = "Sales";
            user2.IsActive = false;
            user2.IsGuest = true;
            user2.SetSourceIdentity("user2@corp.example.com", IdPProviderType.azure_ad);
            user2.AddTag(name: "user2_tag", value: "test");

            // add groups
            IdPGroup group1 = idp.AddGroup(
                name: "group1",
                fullName: "Group 1",
                identity: "g1111"
            );
            IdPGroup group2 = idp.AddGroup(
                name: "group2",
                fullName: "Group 2",
                identity: "g2222"
            );
            IdPGroup group3 = idp.AddGroup(
                name: "group3",
                fullName: "Group 3",
                identity: "g333"
            );
            IdPGroup group4 = idp.AddGroup(
                name: "group4",
                fullName: "Group 4",
                identity: "g4444"
            );

            // add a groug constructed outside of the provider
            IdPGroup group5 = new(
                name: "group5",
                fullName: "Group 5",
                identity: "g5555"
            );
            idp.AddGroup(group5);

            group1.SetProperty("owner", "user1");
            group1.AddAssumedRole("arn:aws:iam::123456789012:role/role1");
            group1.AddTag(name: "group1_tag", value: "test");

            // add users to groups
            idp.Users["1111"].AddGroup(group1);
            idp.Users["2222"].AddGroups(new List<IdPGroup>() {group1, group2});
            idp.Users["3333"].AddGroups(new List<IdPGroup>() {group2, group3});
            idp.Users["4444"].AddGroup(group2);
            idp.Users["5555"].AddGroup(group3);

            // nest groups
            idp.Groups["g4444"].AddGroup(group3);
            idp.Groups["g2222"].AddGroups(new List<IdPGroup>() {group3, group4});
            return idp;
        }
    }
}