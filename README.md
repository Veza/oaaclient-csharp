---
title: Using the C# .NET SDK
description: Building blocks for your custom OAA integration
---

# OAA C# .NET SDK
The `Veza` package provides data models, methods, and helpers for using the [Open Authorization API](https://github.com/Veza). You can use it to populate OAA templates (Application, Filesystem, HRIS, and IdP), push OAA data to Veza, and as a generic Veza API client.

The `Veza` SDK includes the following core components:

* `Veza.Sdk.Client`: A base API client useful for making REST calls to a Veza tenant
* `Veza.OAA.Client`: An OAA API client useful for interacting with integration providers, data sources, and pushing OAA metadata to a Veza tenant.

### What is OAA?

The Open Authorization API is used to submit authorization metadata for custom applications to a Veza instance for parsing and inclusion in the Entity Catalog.

- A typical OAA-based integration will use APIs to query the source application for information about users, resources, and permissions, along with other authorization entities such as groups and roles.
- This data payload is published to Veza as a JSON object. The `oaaclient` modules simplify building the required JSON model and pushing the payload to Veza via the REST API.
- Any application or identity provider added using OAA becomes fully available for search, rules and alerts, and access reviews, similar to any officially-supported integration.

### Building Components

To make use of this repository for your OAA connector:
1. Clone this repository
2. Open `Veza.sln` in Visual Studio
3. Ensure that your Solution Configuration is set to `Release`/`x64`
4. Build the `OAA` and `Client` projects to produce `<output_directory>/net8.0/OAA.dll` and `<output_directory>/net8.0/Sdk.dll`

Once these assemblies are built, import them as dependencies in your project to use the SDK.

### Sample Workflow

Create the Veza API connection and a new custom application:

```c#
using Veza.OAA;
using Veza.OAA.Application;
using Veza.OAA.Base;

// inside namespace/class
            OAAClient oaaClient = new(api_key: <your_api_key>, url: <veza_tenant_url>);
            CustomApplication customApp = new(name: "sample app",
                applicationType: "sample", description: "This is a sample application");
```

Once the `CustomApplication` class is instantiated, you can use its public methods to populate the new app with users, groups, resources, and permissions metadata:

```c#
            // add custom permissions
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

            // define custom user properties
            customApp.DefinedProperties[typeof(User)].DefineProperty("is_guest", typeof(bool));

            // add user
            User user = customApp.AddUser(name: "bob"");
            user.AddIdentity("bob@example.com");
            user.IsActive = true;
            user.CreatedAt = "2001-01-01T00:00:00.000Z".FromRFC3339();
            user.DeactivatedAt = "2003-03-01T00:00:00.000Z".FromRFC3339();
            user.LastLoginAt = "2002-02-01T00:00:00.000Z".FromRFC3339();
            user.PasswordLastChangedAt = "2004-04-01T00:00:00.000Z".FromRFC3339();
            user.SetProperty(name: "is_guest", value: false);
 
            // define group properties
            customApp.DefinedProperties[typeof(Group)].DefineProperty("group_id", typeof(int));
            
            // add group
            Group group1 = customApp.AddGroup("group1");
            group1.CreatedAt = "2001-01-01T00:00:00.000Z".FromRFC3339();
            group1.SetProperty(name: "group_id", 1);
            customApp.Users["bob"].AddGroup("group1");
            Group group2 = customApp.AddGroup("group2");
            group3.AddGroup("group1");

            // idp identities
            customApp.AddIdPIdentity("user01@example.com");

            // define role properties
            customApp.DefinedProperties[typeof(Role)].DefineProperty("custom", typeof(bool));

            // add roles
            Role role1 = customApp.AddRole(name: "role1", permissions: new List<string> { "all", "Admin", "Manage_Thing" });
            role1.SetProperty(name: "custom", value: false);

            // define resource properties
            customApp.DefineResourceProperty("private", typeof(bool), "thing");

            // add resources
            Resource thing1 = customApp.AddResource(name: "thing1", resourceType: "thing", description: "thing1");
            thing1.SetProperty(name: "private", false);
            thing1.AddTag(name: "tag1", value: "This is a value @,-_.");
            Resource cog1 = thing1.AddSubResource(name: "cog1", resourceType: "cog");
            cog1.AddConnection(id: "service_account@some-project.iam.gserviceaccount.com", nodeType: "GoogleCloudServiceAccount");

            // authorizations
            customApp.Users["bob"].AddRole(name: "role1", applyToApplication: true);
            customApp.Groups["group2"].AddRole(name: "role1", resources: new List<Resource> { thing1 });
            customApp.IdPIdentities["user01@example.com"].AddRole(name: "role1", applyToApplication: true);

            return customApp;
```

Once all identities, permissions, and resources are added to the CustomApplication object, use the client connection to push the data to Veza:

```c#
    await oaaClient.CreateProvider(provider_name: "sample app", custom_template: "application");
    await oaaClient.PushApplication(provider_name: "sample app", data_source_name: "sample app 1", customApp);
```

See the [GitHub quickstarts ](https://github.com/Veza/oaa-community/tree/main/quickstarts) directory for complete examples.

### Handling Errors

The `Veza.OAA` namespace provides exception types for common errors that occur when interacting with Veza APIs.

If there are errors interacting with the Veza API, an `OAAClientException` will be raised.

If a provided payload doesn't conform to the template requirements, a `TemplateException` will be raised. The inner exception will carry details about the issues encountered.

### Additional Documentation

Since any given source application or service will have different methods for retrieving entities, authorization, and other required metadata, each OAA connector will be slightly different. You should consult the API documentation for your application when considering how you will source the information, and refer to existing Veza-supported OAA connectors for real-world examples.

Connector source code and `Veza` components are annotated for reference when building your own integrations.

For additional information about developing a custom OAA integration, please see the Veza User Guide.
