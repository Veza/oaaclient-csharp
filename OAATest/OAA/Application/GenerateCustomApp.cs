using Veza.OAA;
using Veza.OAA.Application;
using Veza.OAA.Base;

namespace Veza.OAATest.ApplicationTest
{
    internal static class GenerateCustomApp
    {
        internal static CustomApplication GenerateApp()
        {
            CustomApplication customApp = new(name: "dotnet app",
                applicationType: "dotnet", description: "This is a test");

            // create a custom application
            customApp.DefinedProperties[typeof(CustomApplication)].DefineProperty("version", typeof(string));
            customApp.SetProperty(name: "version", value: "2022.2.2");

            // add custom permissions
            customApp.AddCustomPermission(name: "all", permissions: new List<Permission>
                {
                    Permission.DataCreate,
                    Permission.DataDelete,
                    Permission.DataRead,
                    Permission.DataWrite,
                    Permission.MetadataCreate,
                    Permission.MetadataDelete,
                    Permission.MetadataRead,
                    Permission.MetadataWrite,
                    Permission.NonData
                }
            );
            customApp.AddCustomPermission(name: "Admin", permissions: new List<Permission>
                {
                    Permission.DataRead,
                    Permission.DataWrite,
                    Permission.MetadataRead,
                    Permission.MetadataWrite,
                    Permission.NonData
                }, 
                applyToSubResources: true
            );
            customApp.AddCustomPermission(name: "Manage", permissions: new List<Permission>
                {
                    Permission.DataRead,
                    Permission.DataWrite,
                    Permission.MetadataRead,
                    Permission.MetadataWrite,
                    Permission.NonData
                }
            );
            customApp.AddCustomPermission(name: "View", permissions: new List<Permission>
                {
                    Permission.DataRead,
                    Permission.MetadataRead
                }
            );
            customApp.AddCustomPermission(name: "Manage_Thing", permissions: new List<Permission>
                {
                    Permission.DataRead,
                    Permission.DataWrite
                },
                resourceTypes: new List<string> { "thing" }
            );

            // define user properties
            customApp.DefinedProperties[typeof(User)].DefineProperty("is_guest", typeof(bool));
            customApp.DefinedProperties[typeof(User)].DefineProperty("user_id", typeof(int));
            customApp.DefinedProperties[typeof(User)].DefineProperty("NAME", typeof(string));
            customApp.DefinedProperties[typeof(User)].DefineProperty("peers", typeof(List<string>));
            customApp.DefinedProperties[typeof(User)].DefineProperty("birthday", typeof(DateTime));

            // add users
            List<string> usernames = new() { "bob", "mary", "sue", "rob" };
            foreach( string username in usernames)
            {
                User user = customApp.AddUser(name: username);
                user.AddIdentity($"{username}@example.com");
                user.IsActive = true;
                user.CreatedAt = "2001-01-01T00:00:00.000Z".FromRFC3339();
                user.DeactivatedAt = "2003-03-01T00:00:00.000Z".FromRFC3339();
                user.LastLoginAt = "2002-02-01T00:00:00.000Z".FromRFC3339();
                user.PasswordLastChangedAt = "2004-04-01T00:00:00.000Z".FromRFC3339();
                user.SetProperty(name: "birthday", "2000-01-01T00:00:00.000Z");
                user.SetProperty(name: "is_guest", value: false);
                user.SetProperty(name: "NAME", username.ToUpper());
                user.SetProperty(name: "peers", usernames);
                user.SetProperty(name: "user_id", usernames.IndexOf(username));
            }
            customApp.Users["mary"].IsActive = false;

            // define group properties
            customApp.DefinedProperties[typeof(Group)].DefineProperty("group_id", typeof(int));
            
            // add groups
            Group group1 = customApp.AddGroup("group1");
            group1.CreatedAt = "2001-01-01T00:00:00.000Z".FromRFC3339();
            group1.SetProperty(name: "group_id", 1);
            customApp.Users["bob"].AddGroup("group1");
            customApp.Users["mary"].AddGroup("group1");
            Group group2 = customApp.AddGroup("group2");
            group2.CreatedAt = "2001-01-01T00:00:00.000Z".FromRFC3339();
            group2.SetProperty(name: "group_id", 2);
            customApp.Users["bob"].AddGroup("group2");
            customApp.Users["mary"].AddGroup("group2");
            Group group3 = customApp.AddGroup("group3");
            group3.AddGroup("group1");
            group3.AddGroup("group2");
            customApp.Users["rob"].AddGroup("group3");

            // idp identities
            customApp.AddIdPIdentity("user01@example.com");

            // define role properties
            customApp.DefinedProperties[typeof(Role)].DefineProperty("custom", typeof(bool));
            customApp.DefinedProperties[typeof(Role)].DefineProperty("role_id", typeof(int));

            // add roles
            Role role1 = customApp.AddRole(name: "role1", permissions: new List<string> { "all", "Admin", "Manage_Thing" });
            role1.SetProperty(name: "custom", value: false);
            role1.SetProperty(name: "role_id", value: 1);
            Role role2 = customApp.AddRole(name: "role2");
            role2.AddPermission("view");
            role2.SetProperty("custom", true);
            role2.SetProperty("role_id", 1);
            Role role3 = customApp.AddRole(name: "role3");
            role3.AddPermissions(new List<string> { "manage" });
            role3.SetProperty("role_id", 3);
            role3.AddRole("role2");
            Role _ = customApp.AddRole(name: "empty_role");

            // define resource properties
            customApp.DefineResourceProperty("hair_color", typeof(string), "thing");
            customApp.DefineResourceProperty("peers", typeof(List<string>), "thing");
            customApp.DefineResourceProperty("private", typeof(bool), "thing");
            customApp.DefineResourceProperty("publish_date", typeof(DateTime), "thing");
            customApp.DefineResourceProperty("resource_id", typeof(int), "thing");

            // add resources
            Resource thing1 = customApp.AddResource(name: "thing1", resourceType: "thing", description: "thing1");
            thing1.SetProperty(name: "private", false);
            thing1.SetProperty("hair_color", "blue");
            thing1.SetProperty("peers", new List<string> { "thing2", "thing3" });
            thing1.SetProperty("publish_date", "1959-03-12T00:00:00.000Z");
            thing1.SetProperty("resource_id", 1);
            thing1.AddTag(name: "tag1", value: "This is a value @,-_.");
            Resource thing2 = customApp.AddResource(name: "thing2", resourceType: "thing");
            thing2.SetProperty(name: "private", false);
            thing2.SetProperty("hair_color", "blue");
            thing2.SetProperty("peers", new List<string> { "thing2", "thing3" });
            thing2.SetProperty("publish_date", "1959-03-12T00:00:00.000Z");
            thing2.SetProperty("resource_id", 2);
            Resource cog1 = thing2.AddSubResource(name: "cog1", resourceType: "cog");
            cog1.AddConnection(id: "service_account@some-project.iam.gserviceaccount.com", nodeType: "GoogleCloudServiceAccount");

            // authorizations
            customApp.Users["bob"].AddRole(name: "role1", applyToApplication: true);
            customApp.Users["sue"].AddRole(name: "role3", applyToApplication: true);
            customApp.Groups["group2"].AddRole(name: "role2", resources: new List<Resource> { thing1 });
            customApp.Users["mary"].AddPermission("view", resources: new List<Resource> { thing2, cog1 });
            customApp.Users["rob"].AddPermission("manage", resources: new List<Resource> { thing1 }, applyToApplication: true);
            customApp.IdPIdentities["user01@example.com"].AddRole(name: "role1", applyToApplication: true);

            return customApp;

        }
    }
}
