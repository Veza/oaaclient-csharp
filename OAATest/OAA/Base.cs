using Microsoft.Extensions.Configuration;
using System.Reflection;
using Veza.OAATest.ClientTest;

namespace Veza.OAATest
{
    public class Base
    {
        public static void SetEnvironmentVariablesFromSecrets()
        {
            var configuration = new ConfigurationBuilder().AddUserSecrets<ProviderTest>().Build();
            foreach (var child in configuration.GetChildren())
            {
                Environment.SetEnvironmentVariable(child.Key, child.Value);
            }
        }

        internal static string ReadEmbeddedResource(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = resourceName;
            using Stream? stream = asm.GetManifestResourceStream(resource);
            if (stream is not null)
            {
                StreamReader r = new StreamReader(stream);
                return r.ReadToEnd();
            }
            else
            {
                return "";
            }
        }
    }
}
