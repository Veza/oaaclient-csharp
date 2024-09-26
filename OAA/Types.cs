namespace Veza.OAA
{
    /// <summary>
    /// IdP entity types.
    /// </summary>
    public enum IdPEntityType
    {
        DOMAIN,
        GROUP,
        USER
    }

    /// <summary>
    /// Veza-supported IdP provider types.
    /// </summary>
    public enum IdPProviderType
    {
        active_directory,
        any,
        azure_ad,
        custom,
        google_workspace,
        okta,
        one_login
    }

    /// <summary>
    /// Types of identities for permissions mapping.
    /// </summary>
    public enum IdentityType
    {
        local_user,
        local_group,
        local_role,
        idp
    }

    /// <summary>
    /// Canonical permissions used by the Veza Open Authorization Framework
    /// </summary>
    public enum Permission
    {
        DataCreate,
        DataDelete,
        DataRead,
        DataWrite,
        MetadataCreate,
        MetadataDelete,
        MetadataRead,
        MetadataWrite,
        NonData,
        Uncategorized
    }

    /// <summary>
    /// Supported types for custom properties on entities (application, resource, identity)
    /// </summary>
    public enum PropertyType
    {
        BOOLEAN,
        NUMBER,
        STRING,
        STRING_LIST,
        TIMESTAMP
    }
}
