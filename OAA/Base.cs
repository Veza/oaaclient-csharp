using System.Text.Json;
using System.Text.RegularExpressions;
using Veza.OAA.Exceptions;

namespace Veza.OAA.Base
{
    /// <summary>
    /// Base Abstract class for entities that can be added to Veza
    /// </summary>
    public abstract class VezaEntity
    {
        public string? Description { get; set; }
        internal Type EntityType { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public PropertyDefinitions? PropertyDefinitions { get; set; }
        public List<Tag> Tags { get; set; }
        public string? UniqueId { get; set; }

        internal VezaEntity(
            string name,
            Type entityType,
            string? description = null,
            Dictionary<string, object>? properties = null,
            PropertyDefinitions? propertyDefinitions = null,
            List<Tag>? tags = null,
            string? uniqueId = null
            )
        {
            Description = description;
            EntityType = entityType;
            Name = name;
            Properties = properties ?? [];
            PropertyDefinitions = propertyDefinitions;
            Tags = tags ?? [];
            UniqueId = uniqueId ?? name;
        }

        /// <summary>
        /// Add a tag to a Veza entity
        /// </summary>
        /// <param name="name">The string tag name</param>
        public void AddTag(string name)
        {
            Tag tag = new(name);
            if (!Tags.Any(t => t.Key == name))
            {
                Tags.Add(tag);
            }
        }

        /// <summary>
        /// Add a tag to a Veza entity
        /// </summary>
        /// <param name="name">The string tag name</param>
        /// <param name="value">The string tag value</param>
        public void AddTag(string name, string value)
        {
            Tag tag = new(name, value);
            if (!Tags.Any(t => t.Key == name && t.Value == value))
            {
                Tags.Add(tag);
            }
        }

        /// <summary>
        /// Add an existing tag to the role
        /// </summary>
        /// <param name="tag">The tag object to add to the role</param>
        public void AddTag(Tag tag)
        {
            if (!Tags.Any(t => t.Key == tag.Key && t.Value == tag.Value))
            {
                Tags.Add(tag);
            }
        }

        /// <summary>
        /// Set a property on the derived object
        /// </summary>
        /// <param name="name">The string name of the property</param>
        /// <param name="value">The object value of the property</param>
        /// <exception cref="TemplateException">Throws if custom properties have not been defined on the EntityType</exception>
        public void SetProperty(string name, object value)
        {
            if (PropertyDefinitions == null)
            {
                throw new TemplateException($"No custom properties defined; cannot set value");
            }

            if (PropertyDefinitions.ValidateProperty(name))
            {
                Properties.Add(name, value);
            }
            else
            {
                throw new TemplateException($"Custom property {name} not defined on entity {EntityType}; cannot set value");
            }
        }
    }

    /// <summary>
    /// Model for defining custom properties for a Veza entity
    /// 
    /// Property definitions specify the names and types of custom 
    /// properties to be assigned to an entity
    /// </summary>
    public class PropertyDefinitions
    {
        internal Dictionary<string, PropertyType> Properties {  get; set; }

        internal PropertyDefinitions()
        {
            Properties = [];
        }

        /// <summary>
        /// Define a property
        /// </summary>
        /// <param name="name">The string name of the property</param>
        /// <param name="propertyType">The C# type of the property</param>
        public void DefineProperty(string name, Type propertyType)
        {
            ValidatePropertyName(name);

            Properties[name] = propertyType switch
            {
                Type _ when propertyType == typeof(bool) => PropertyType.BOOLEAN,
                Type _ when propertyType == typeof(int) => PropertyType.NUMBER,
                Type _ when propertyType == typeof(string) => PropertyType.STRING,
                Type _ when propertyType == typeof(List<string>) => PropertyType.STRING_LIST,
                Type _ when propertyType == typeof(DateTime) => PropertyType.TIMESTAMP,
                _ => throw new ArgumentException($"Veza property type not defined for {propertyType.GetType()}"),
            };
        }

        /// <summary>
        /// Convert the PropertyDefinitions object to a Dictionary of strings for serialization
        /// </summary>
        /// <returns>A Dictionary representation of the object</returns>
        internal Dictionary<string, string> ToDictionary()
        {
            return Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
        }

        /// <summary>
        /// Validate that a property name has been defined for the given entity type
        /// and can be assigned a value
        /// </summary>
        /// <param name="name">The string name of the property</param>
        /// <returns>A boolean indicating if the property name is defined</returns>
        internal bool ValidateProperty(string name)
        {
            List<string> validPropertyNames = Properties.Keys.ToList();
            List<string> lowerValidPropertyNames = [];

            foreach (string validName in validPropertyNames)
            {
                lowerValidPropertyNames.Add(validName.ToLower());
            }

            if (lowerValidPropertyNames.Contains(name.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validate a string property name against Veza-allowed characters
        /// </summary>
        /// <param name="name">The string property name</param>
        /// <exception cref="TemplateException">Throws if the name contains illegal characters</exception>
        internal static void ValidatePropertyName(string name)
        {
            Match match = Regex.Match(name.ToLower(), @"^[a-z][a-z_]*$");
            if (!match.Success) { throw new TemplateException("Lower-cased property name must match the regex pattern: ^[a-z][a-z_]*$"); }
        }
    }

    /// <summary>
    /// Base class for CustomProvider
    /// </summary>
    public abstract class Provider
    {
        public string Name { get; set; }
        public string Template { get; set; }

        public Provider(string name, string template)
        {
            Name = name;
            Template = template;
        }
    }

    /// <summary>
    /// Veza tag data model
    /// </summary>
    public class Tag
    {
        public string Key { get; set; }
        public string? Value { get; set; }

        public Tag(string key)
        {
            // Ensure the tag key contains only characters accepted by the Veza platform
            Match key_match = Regex.Match(key, @"^[\w\d_]+$");
            if (!key_match.Success)
            {
                throw new TemplateException(
                    $"Invalid characters in tag key {key}: may only contain letters, numbers, whitespace, and underscore"
                );
            }
            Key = key;
        }

        public Tag(string key, string? value = null)
        {
            // Ensure the tag key contains only characters accepted by the Veza platform
            Match key_match = Regex.Match(key, @"^[\w\d_]+$");
            if (!key_match.Success)
            {
                throw new TemplateException(
                    $"Invalid characters in tag key {key}: may only contain letters, numbers, whitespace, and underscore"
                );
            }
            Key = key;

            // Ensure the tag value contains only characters accepted by the Veza platform
            if (value != null)
            {
                Match value_match = Regex.Match(value, @"^[\w\d\s_,@\.-]+$");
                if (!value_match.Success)
                {
                    throw new TemplateException(
                        $"Invalid characters in tag value {value}; may only contain letters, numbers, whitespace, and the @,._- special characters"
                    );
                }
                Value = value;
            }
        }

        /// <summary>
        /// Return a dictionary representation of the Tag
        /// </summary>
        /// <returns>
        /// A dictionary representation of the Tag
        /// </returns>
        public Dictionary<string, object> ToDictionary()
        {
            if (Value is not null)
            {
                return new Dictionary<string, object>() 
                { 
                    { "Key", Key },
                    { "Value", Value } 
                };
            }
            else
            {
                return new Dictionary<string, object> { { "Key", Key } };
            }
            
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Return a string representation of the Tag
        /// </summary>
        /// <returns>
        /// A string representation of the Tag
        /// </returns>
        public override string ToString()
        {
            if (Value != null)
            { return $"Tag {Key}: {Value}"; }
            else
            { return $"Tag {Key}"; }
        }

        /// <summary>
        /// Test if the given key and value are the same as an existing tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool Equals(string key, string value)
        {
            if (key == Key && value == Value)
            { return true; }
            else
            { return false; }
        }

        public bool Equals(Tag tag)
        {
            if (Value is not null)
            { return Key == tag.Key && Value == tag.Value; }
            else
            { return Key == tag.Key; }
        }
    }

}
