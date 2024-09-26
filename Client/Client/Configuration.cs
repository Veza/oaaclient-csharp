using System.Collections.Concurrent;
using System.Diagnostics;
using Veza.Sdk.Exceptions;

namespace Veza.Sdk.Client
{
    public interface IConfiguration
    {
        string ApiKey { get; }
        IDictionary<string, string> DefaultHeaders { get; }
        bool EnableCompression { get; }
        int? RetryMaxBackoff { get; }
        int? MaxRetries { get; }
        int? PageSize { get; }
        double? RetryBackoff { get; }
        int Timeout { get; }
        string UserAgent { get; }
        bool VerifySSL { get; }
        Uri? URI { get; }
        string URL { get; }
    }

    public class Configuration : IConfiguration
    {
        #region Constants
        public const int DefaultMaxRetries = 10;
        public const int DefaultPageSize = 250;
        public const double DefaultRetryBackoff = 0.6;
        public const int DefaultRetryMaxBackoff = 30;
        public const int DefaultTimeout = 30000;
        public const string DefaultUserAgent = "vezaclient/1.0 csharp/12.0 windows";
        #endregion Constants


        #region Members
        public string ApiKey { get; set; }

        public virtual IDictionary<string, string> DefaultHeaders { get; set; }
        /// <summary>
        /// Allow compression for the client connection
        /// </summary>
        public bool EnableCompression { get; set; } = true;

        public int? MaxRetries { get; set; } = DefaultMaxRetries;
        public int? PageSize { get; set; } = DefaultPageSize;
        public double? RetryBackoff { get; set; } = DefaultRetryBackoff;
        public int? RetryMaxBackoff { get; set; } = DefaultRetryMaxBackoff;
        public int Timeout { get; set; } = DefaultTimeout;
        public Uri? URI { get; set; }
        public string URL { get; set; }
        public virtual string UserAgent { get; set; } = DefaultUserAgent;

        /// <summary>
        /// Enable/Disable certificate checking.
        /// Note: Setting this to false is not recommended for production use.
        /// </summary>
        private bool _verify_ssl = true;
        public bool VerifySSL
        {
            get { return _verify_ssl; }
            set
            {
                if (value) { Trace.TraceWarning("SSL Verification disabled. This is insecure and should not be used in production"); }
                _verify_ssl = value;
            }
        }
        #endregion Members


        #region Static Members
        /// <summary>
        /// Validate the config and throw exceptions for common misconfigurations
        /// </summary>
        /// <param name="config">The configuration object</param>
        /// <exception cref="ClientException">Thrown when a misconfiguration is present</exception>
        public static void Validate(IConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(config.ApiKey))
            {
                throw new ClientException("API key validation failed", "The provided API key is empty.");
            }

            if (config.VerifySSL && !config.URL.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                throw new ClientException("URL validation failed", $"Veza URL must begin with https. Current value: {config.URL}");
            }
        }
        #endregion Static Members


        #region Constructors
        public Configuration()
        {
            DefaultHeaders = new Dictionary<string, string>();
            EnableCompression = true;
            ApiKey = string.Empty;
            URL = string.Empty;
        }

        public Configuration(string api_key, string url)
        {
            URL = Util.ParseURL(url);
            URI = new Uri(URL, UriKind.Absolute);

            if (string.IsNullOrWhiteSpace(api_key))
            {
                throw new ClientException("API key validation failed", "The provided API key is empty.");
            }
            ApiKey = api_key;
            DefaultHeaders = new ConcurrentDictionary<string, string>();
            EnableCompression = true;
        }
        public Configuration(string api_key, string url, Dictionary<string, string> headers) : this()
        {
            URL = Util.ParseURL(url);
            URI = new Uri(URL, UriKind.Absolute);
            if (string.IsNullOrWhiteSpace(api_key))
            {
                throw new ClientException("API key validation failed", "The provided API key is empty.");
            }
            ApiKey = api_key;
            foreach (var kvp in headers)
            {
                DefaultHeaders.Add(kvp);
            }
            EnableCompression = true;
        }
        #endregion Constructors
    }
}
