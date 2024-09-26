using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Net.Sockets;
using Veza.Sdk.Exceptions;

namespace Veza.Sdk.Client
{
    public partial class ApiClient : IDisposable
    {
        #region Members

        public const string AllowedCharacters = @"^[ @#$%&*:()!,a-zA-Z0-9_'""=.-]*$";
        public const int ProviderIconMaxSize = 64000;
        readonly RestClient _client;
        public Configuration configuration;
        public record VezaApiResponse(Dictionary<string, object> value, List<Dictionary<string, object>> values);

        #endregion Members

        #region Constructors
        public ApiClient(string api_key, string url)
        {
            configuration = new(api_key, url);
            JwtAuthenticator auth = new(api_key);
            RestClientOptions options = new()
            {
                Authenticator = auth,
                BaseUrl = configuration.URI,
                UserAgent = configuration.UserAgent,
                Timeout = TimeSpan.FromSeconds(configuration.Timeout)
            };
            _client = new RestClient(options);
            TestVezaEndpoint();
        }

        #endregion Constructors

        #region Base API interactions

        /// <summary>
        /// Base Async API DELETE Request
        /// </summary>
        /// <param name="path">The path for the request</param>
        /// <param name="query_params">Query parameters for the request</param>
        /// <param name="header_params">Headers for the request</param>
        /// <param name="path_params">Path parameters for the request</param>
        /// <returns>A Task containing the HTTP DELETE response object</returns>
        public async Task<RestResponse<VezaApiResponse>> ApiDelete(
            string path,
            List<KeyValuePair<string, string>>? query_params = null,
            Dictionary<string, string>? header_params = null,
            Dictionary<string, string>? path_params = null
            )
        {
            RestRequest request = NewRequest(path,
                Method.Delete,
                query_params ?? [],
                header_params ?? [],
                [],
                path_params ?? [],
                null);
            var response = await _client.ExecuteAsync<VezaApiResponse>(request);
            return response;
        }

        /// <summary>
        /// Base Async API GET Request
        /// </summary>
        /// <param name="path">The path for the request</param>
        /// <param name="query_params">Query parameters for the request</param>
        /// <param name="header_params">Headers for the request</param>
        /// <param name="path_params">Path parameters for the request</param>
        /// <returns>A Task containing the HTTP GET response object</returns>
        public async Task<RestResponse<VezaApiResponse>> ApiGet(
            string path,
            List<KeyValuePair<string, string>>? query_params = null,
            Dictionary<string, string>? header_params = null,
            Dictionary<string, string>? path_params = null
            )
        {
            RestRequest request = NewRequest(path,
                Method.Get,
                query_params ?? [],
                header_params ?? [],
                [],
                path_params ?? [],
                null);
            var response = await _client.ExecuteAsync<VezaApiResponse>(request);

            return response;
        }


        /// <summary>
        /// Base Async API POST Request
        /// </summary>
        /// <param name="path">The path for the request</param>
        /// <param name="query_params">Query parameters for the request</param>
        /// <param name="header_params">Headers for the request</param>
        /// <param name="path_params">Path parameters for the request</param>
        /// <returns>A Task containing the HTTP POST response object</returns>
        public async Task<RestResponse<VezaApiResponse>> ApiPost(
        string path,
        object data,
        List<KeyValuePair<string, string>>? query_params = null,
        Dictionary<string, string>? header_params = null,
        Dictionary<string, string>? path_params = null
        )
        {
            RestRequest request = NewRequest(path,
                Method.Post,
                query_params ?? [],
                header_params ?? [],
                [],
                path_params ?? [],
                data);
            var response = await _client.ExecuteAsync<VezaApiResponse>(request);
            return response;
        }

        /// <summary>
        /// Base Async API PUT Request
        /// </summary>
        /// <param name="path">The path for the request</param>
        /// <param name="query_params">Query parameters for the request</param>
        /// <param name="header_params">Headers for the request</param>
        /// <param name="path_params">Path parameters for the request</param>
        /// <returns>A Task containing the HTTP POST response object</returns>
        public async Task<RestResponse<VezaApiResponse>> ApiPut(
            string path,
            object data,
            List<KeyValuePair<string, string>>? query_params = null,
            Dictionary<string, string>? header_params = null,
            Dictionary<string, string>? path_params = null
            )
        {
            RestRequest request = NewRequest(path,
                Method.Put,
                query_params ?? [],
                header_params ?? [],
                [],
                path_params ?? [],
                data);
            var response = await _client.ExecuteAsync<VezaApiResponse>(request);
            return response;
        }

        #endregion Base API interactions

        #region Util

        /// <summary>
        /// Resolve the Veza Endpoint
        /// Dns.GetHostEntry often can fail and throw a SocketException if a valid name hasn't been resolved recently - 
        /// this function is designed to be run in a loop to retry in those cases. 
        /// 
        /// Other failures (bad hostname, other cases) will throw immediately.
        /// </summary>
        /// <param name="attempt">The int number of times the resolution has been attempted</param>
        /// <returns>A boolean indicating whether the resolution was successful</returns>
        /// <exception cref="ClientException">Throws if DNS resolution fails</exception>
        internal bool ResolveVezaEndpoint(int attempt = 1)
        {
            try
            {
                _ = Dns.GetHostEntry(configuration.URI.Host);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ClientException(error: "URL invalid", message: "The provided URL is greater than 255 characters");
            }
            catch (SocketException)
            {
                if (attempt >= 5)
                    throw new ClientException(error: "DNS resolution error", message: "The Veza URL provided could not be resolved");
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new ClientException(error: "Unknown DNS error", message: ex.Message);
            }

            return true;
        }

        /// <summary>
        /// Test the Veza Client connection
        /// 
        /// Verify that the fqdn can be resolved and that a GET to the provider templates endpoint succeeds
        /// </summary>
        internal void TestVezaEndpoint()
        {
            // Resolve the endpoint; loop to avoid spurious Dns.GetHostEntry failures
            bool endpoint_resolved = false;
            for (int i=1; i <= 6; i++)
            {
                if (!endpoint_resolved)
                    endpoint_resolved = ResolveVezaEndpoint(i);
                else
                    break;
            }

            // test a GET to the Veza instance to validate credentials
            RestResponse res = ApiGet("/api/v1/providers/custom/templates").Result;
            if (!res.IsSuccessful)
            {
                if (res.ErrorMessage != null)
                {
                    throw new ClientException("API Credentials verification failed", res.ErrorMessage);
                }
                else
                {
                    throw new ClientException("API Credentials verification failed", string.Empty);
                }
            }
        }

        #endregion Util

        #region RestSharp Helpers

        /// <summary>
        /// Dispose of the client (and the underlying HttpClient)
        /// </summary>
        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Create a RestRequest
        /// </summary>
        /// <param name="path">The path for the request</param>
        /// <param name="method">The HTTP Method for the request</param>
        /// <param name="query_params">Query parameters for the request</param>
        /// <param name="header_params">Headers for the request</param>
        /// <param name="form_params"></param>
        /// <param name="path_params">Path parameters for the request</param>
        /// <param name="body">Data for POST/PUT requests</param>
        /// <returns>The RestRequest object to be executed</returns>
        private static RestRequest NewRequest(
            string path,
            RestSharp.Method method,
            List<KeyValuePair<string, string>> query_params,
            Dictionary<string, string> header_params,
            Dictionary<string, string> form_params,
            Dictionary<string, string> path_params,
            object? body
            )
        {
            RestRequest request = new(path, method);

            // add path parameters
            foreach (var param in path_params)
                request.AddParameter(param.Key, param.Value, ParameterType.UrlSegment);

            // add headers
            foreach (var header in header_params)
                request.AddHeader(header.Key, header.Value);

            // add query parameters
            foreach (var param in query_params)
                request.AddQueryParameter(param.Key, param.Value);

            // add form parameters
            foreach (var param in form_params)
                request.AddParameter(param.Key, param.Value);

            if (body != null)
                request.AddJsonBody(body);
            return request;
        }

        #endregion RestSharp Helpers
    }
}
