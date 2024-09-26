using RestSharp;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Veza.OAA.Application;
using Veza.OAA.Exceptions;
using Veza.OAA.HRIS;
using Veza.OAA.IdP;
using Veza.Sdk.Client;
using Veza.Sdk.Exceptions;

namespace Veza.OAA.Client
{
    public class OAAClient(string api_key, string url) : ApiClient(api_key, url)
    {
        public const int PayloadMaxSize = 100000000;

        #region DataSource

        /// <summary>
        /// Create a new Data Source for the provided provider_id
        /// </summary>
        /// <param name="data_source_name">The string name for the new data source</param>
        /// <param name="provider_id">The string unique identifier for the provider</param>
        /// <param name="options">Additional options to be included with the data source create call</param>
        /// <returns>The result of the create data source API POST request</returns>
        /// <exception cref="ArgumentException">Thrown if the data source name contains invalid characters</exception>
        public async Task<RestResponse<VezaApiResponse>> CreateDataSource(string data_source_name, string provider_id, Dictionary<string, string>? options = null)
        {
            Match match = Regex.Match(data_source_name, AllowedCharacters);
            if (!match.Success)
                throw new ArgumentException(paramName: data_source_name, message: "Provider name contains invalid characters");

            Dictionary<string, string> data_source = new()
            {
                { "name", data_source_name },
                { "id", provider_id }
            };

            // iterate extra options and add them to the data source data
            if (options != null)
            {
                foreach (KeyValuePair<string, string> kvp in options)
                {
                    data_source.Add(kvp.Key, kvp.Value);
                }
            }
            
            Debug.WriteLine($"Creating Data Source {data_source_name} on Provider ID {provider_id}");
            return await ApiPost($"/api/v1/providers/custom/{provider_id}/datasources",
                data_source);
        }

        /// <summary>
        /// Delete a Data Source on the provided provider_id
        /// </summary>
        /// <param name="data_source_id">The string unique identifier for the data source</param>
        /// <param name="provider_id">The string unique identifier for the provider</param>
        /// <returns>The result of the delete data source API DELETE request</returns>
        public async Task<RestResponse<VezaApiResponse>> DeleteDataSource(string data_source_id, string provider_id)
        {
            Debug.WriteLine($"Deleting Data Source {data_source_id} on Provider {provider_id}");
            return await ApiDelete($"/api/v1/providers/custom/{provider_id}/datasources/{data_source_id}");
        }

        /// <summary>
        /// Get a Data Source by name or ID
        /// </summary>
        /// <param name="provider_id">The string unique identifier for the provider</param>
        /// <param name="data_source_name">The optional string name of the data source to get from Veza</param>
        /// <param name="data_source_id">The optional string unique identifier for the data source to get from Veza</param>
        /// <returns>The result of the get data source API GET request</returns>
        public async Task<Dictionary<string, object>> GetDataSource(string provider_id, string? data_source_name = null, string? data_source_id = null)
        {
            if (!string.IsNullOrEmpty(data_source_name))
            {
                return await GetDataSourceByName(data_source_name, provider_id);
            }
            else
            {
                if (!string.IsNullOrEmpty(data_source_id))
                {
                    return await GetDataSourceById(data_source_id, provider_id);
                }
                else
                {
                    throw new ArgumentException("GetDataSource called without a data_source_id or data_source_name; one is required");
                }
            }
        }

        /// <summary>
        /// Get a Data Source by ID
        /// </summary>
        /// <param name="data_source_id">The string unique identifier for the data source</param>
        /// <param name="provider_id">The string unique identifier for the provider</param>
        /// <returns>The result of the get data source API GET request</returns>
        /// <exception cref="ClientException">Thrown if more than one data source is returned by Veza</exception>
        public async Task<Dictionary<string, object>> GetDataSourceById(string data_source_id, string provider_id)
        {
            Debug.WriteLine($"Getting Data Source {data_source_id} on Provider ID {provider_id}");
            var response = await ApiGet($"/api/v1/providers/custom/{provider_id}/datasources/{data_source_id}");

            if (response.Data is null)
                return [];

            if (response.Data.value.Count == 0)
                return [];
            else
                return response.Data.value;
        }

        /// <summary>
        /// Get a Data Source by name
        /// </summary>
        /// <param name="data_source_name">The string name of the data source to get from Veza</param>
        /// <param name="provider_id">The string unique identifier for the provider</param>
        /// <returns>The result of the get data source API GET request</returns>
        /// <exception cref="ClientException">Thrown if more than one data source is returned by Veza</exception>
        public async Task<Dictionary<string, object>> GetDataSourceByName(string data_source_name, string provider_id)
        {
            KeyValuePair<string, string> filter = new("filter", $"name eq \"{data_source_name}\"");
            Debug.WriteLine($"Getting Data Source {data_source_name} on Provider ID {provider_id}");
            var response = await ApiGet($"/api/v1/providers/custom/{provider_id}/datasources",
                [filter]);

            if (response.Data is null)
                return [];

            if (response.Data.values.Count == 0)
                return [];
            else if (response.Data.values.Count == 1)
                return response.Data.values.First();
            else
                throw new ClientException("Unexpected results", "Datasource list returned more than one result");
        }

        #endregion DataSource

        #region Provider

        /// <summary>
        /// Create a new Provider
        /// </summary>
        /// <param name="provider_name">The string name of the provider</param>
        /// <param name="custom_template">The string name of the OAA template to use for the Provider ("application," "file_system," or "identity_provider")</param>
        /// <param name="base64_icon">The base64-encoded string to use as the provider icon</param>
        /// <param name="options">Additional options to be included with the provider create call</param>
        /// <returns>The result of the create provider API POST request</returns>
        /// <exception cref="ArgumentException">Thrown if the provider name contains invalid characters</exception>
        public async Task<RestResponse<VezaApiResponse>> CreateProvider(string provider_name, string custom_template, string? base64_icon = null, 
                                                                        Dictionary<string, string>? options = null)
        {
            Match match = Regex.Match(provider_name, AllowedCharacters);
            if (!match.Success)
                throw new ArgumentException(paramName: provider_name, message: "Provider name contains invalid characters");

            string provider_id = string.Empty;
            Dictionary<string, string> provider = new()
            {
                { "name", provider_name },
                { "custom_template", custom_template }
            };

            // iterate extra options and add them to the provider data
            if (options != null)
            {
                foreach (KeyValuePair<string, string> kvp in options)
                {
                    provider.Add(kvp.Key, kvp.Value);
                }
            }

            // create the provider
            Debug.WriteLine($"Creating Provider {provider_name}");
            var response = await ApiPost($"/api/v1/providers/custom", provider);
            if (response.IsSuccessful)
            {
                provider_id = response.Data.value["id"].ToString();
            }
            Debug.WriteLine($"Provider ID {provider_id} created");

            // add the provider icon if one is provided
            Debug.WriteLine($"Updating provider icon for provider id {provider_id}");
            if (!string.IsNullOrEmpty(base64_icon))
            {
                RestResponse<VezaApiResponse> _ = await UpdateProviderIcon(provider_id, base64_icon);
            }

            return response;
        }

        /// <summary>
        /// Delete a provider
        /// </summary>
        /// <param name="provider_id">The string id of the provider to be deleted</param>
        /// <returns>The result of the delete provider API DELETE request</returns>
        /// <exception cref="ClientException">Throws if the provider can not be found</exception>
        public async Task<RestResponse<VezaApiResponse>> DeleteProvider(string provider_id)
        {
            Debug.WriteLine($"Deleting provider id {provider_id}");
            return await ApiDelete($"/api/v1/providers/custom/{provider_id}");
        }

        /// <summary>
        /// Get all custom providers from Veza
        /// </summary>
        public async Task<RestResponse<VezaApiResponse>> GetProviders()
        {
            return await ApiGet("/api/v1/providers/custom");
        }

        /// <summary>
        /// Get a provider by name or ID
        /// If only one argument is provided, default to name-based lookup
        /// </summary>
        /// <param name="provider_name">The string name of the provider to get from Veza</param>
        /// <param name="provider_id">The string id of the provider to get from Veza</param>
        /// <returns>The result of the get provider API GET request</returns>
        /// <exception cref="ArgumentException">Thrown if called without a provider_id or provider_name</exception>
        public async Task<VezaApiResponse> GetProvider(string? provider_name = null, string? provider_id = null)
        {
            if (!string.IsNullOrEmpty(provider_name))
            {
                return await GetProviderByName(provider_name);
            }
            else
            {
                if (!string.IsNullOrEmpty(provider_id))
                {
                    return await GetProviderById(provider_id);
                }
                else
                {
                    throw new ArgumentException("GetProvider called without a provider_id or provider_name; one is required");
                }
            }
        }

        /// <summary>
        /// Get a provider by ID
        /// </summary>
        /// <param name="provider_id">The string id of the provider to get from Veza</param>
        /// <returns>The result of the get provider API GET request</returns>
        /// <exception cref="ClientException">Thrown if more than one provider is returned by Veza</exception>
        public async Task<VezaApiResponse> GetProviderById(string provider_id)
        {
            Debug.WriteLine($"Getting provider by ID {provider_id}");
            var response = await ApiGet($"/api/v1/providers/custom/{provider_id}");

            if (response.Data is null)
                return null;

            if (response.Data.value.Count == 0)
            {
                Debug.WriteLine($"No provider with ID {provider_id} found");
                return null;
            }
            else
            {
                Debug.WriteLine("Provider found");
                return response.Data;
            }

        }

        /// <summary>
        /// Get a provider by name
        /// </summary>
        /// <param name="provider_name">The string name of the provider to get from Veza</param>
        /// <returns>The result of the get provider API GET request</returns>
        /// <exception cref="ClientException">Thrown if more than one provider is returned by Veza</exception>
        public async Task<VezaApiResponse> GetProviderByName(string provider_name)
        {
            KeyValuePair<string, string> filter = new("filter", $"name eq \"{provider_name}\"");
            Debug.WriteLine($"Getting provider {provider_name}");
            var response = await ApiGet("/api/v1/providers/custom", [filter]);

            if (response.Data is null)
                return null;
                
            if (response.Data.values.Count == 0)
            {
                Debug.WriteLine($"No provider with name {provider_name} found");
                return null;
            }
            else if (response.Data.values.Count == 1)
            {
                Debug.WriteLine("Provider found");
                return response.Data;
            }
            else
                throw new ClientException("Unexpected results", "Provider list returned more than one result");
        }

        /// <summary>
        /// Update an existing custom provider icon
        /// </summary>
        /// <param name="provider_id">The string provider_id of the existing provider</param>
        /// <param name="base64_icon">The base64-encoded string icon data</param>
        /// <returns>The result of the update icon API POST request</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the icon data exceeds the Veza limit of 64KB</exception>
        public async Task<RestResponse<VezaApiResponse>> UpdateProviderIcon(string provider_id, string base64_icon)
        {
            if (Encoding.Unicode.GetByteCount(base64_icon) > ProviderIconMaxSize)
                throw new ArgumentOutOfRangeException(paramName: nameof(base64_icon), message: "Max icon size of 64KB exceeded");

            Dictionary<string, string> payload = new()
            {
                { "icon_base64", base64_icon }
            };

            return await ApiPost($"/api/v1/providers/custom/{provider_id}:icon", payload);
        }

        #endregion Provider

        #region Application
        /// <summary>
        /// Push an OAA Application object to Veza
        /// 
        /// Extracts the JSON payload from the provided OAA class and pushes the metadata to the provided data_source
        /// 
        /// The provider must be valid and created before PushApplication is called.
        /// 
        /// </summary>
        /// <param name="provider_name">The string name of an existing provider</param>
        /// <param name="data_source_name">The string name of the data_source (will be created if it doesn't exist)</param>
        /// <param name="application">The CustomApplication object representing the OAA payload</param>
        /// <param name="save_json">Boolean flag that writes the JSON payload to a file before push. Defaults to false.</param>
        /// <returns></returns>
        public async Task<RestResponse<VezaApiResponse>> PushApplication(
            string provider_name, 
            string data_source_name, 
            CustomApplication application, 
            bool save_json = false
        )
        {
            string json_metadata = application.GetJSONPayload();
            Debug.WriteLine($"Pushing application payload for provider {provider_name} to data source {data_source_name}");
            return await PushMetadata(provider_name, data_source_name, 
                json_metadata, save_json);
        }
        #endregion

        #region Filesystem
        /// <summary>
        /// Push an OAA Filesystem server object to Veza
        /// 
        /// Extracts the JSON payload from the provided OAA filesystem class and pushes the metadata to the provided data_source
        /// 
        /// The provider must be valid and created before PushServer is called
        /// </summary>
        /// <param name="provider_name">The string name of an existing provider</param>
        /// <param name="data_source_name">The string name of the data_source (will be created if it does not exist</param>
        /// <param name="filesystem">The Filesystem object</param>
        /// <param name="save_json">Boolean flag that writes the JSON payload to a file before push.</param>
        /// <returns>The result of the metadata push API request</returns>
        public async Task<RestResponse<VezaApiResponse>> PushServer(
            string provider_name, 
            string data_source_name, 
            Filesystem.Filesystem filesystem, 
            bool save_json = false
        )
        {
            string json_metadata = filesystem.ServerJSON();
            return await PushMetadata(
                provider_name: provider_name, 
                data_source_name: data_source_name, 
                metadata: json_metadata, 
                save_json: save_json);
        }

        /// <summary>
        /// Push an OAA Filesystem folder payload to Veza
        /// 
        /// Requires an existing provider and data_source
        /// </summary>
        /// <param name="provider_name">The string name of an existing provider</param>
        /// <param name="data_source_name">The string name of an existing data_source</param>
        /// <param name="filesystem">The Filesystem object</param>
        /// <param name="save_json">Boolean flag that writes the JSON payload to a file before push</param>
        /// <returns>The result of the metadata push API request</returns>
        public async Task<RestResponse<VezaApiResponse>> PushFolder(
            string provider_name,
            string data_source_name,
            Filesystem.Filesystem filesystem,
            bool save_json = false
        )
        {
            string json_metadata = filesystem.FolderJSON();
            return await PushMetadata(
                provider_name: provider_name,
                data_source_name: data_source_name,
                metadata: json_metadata,
                save_json: save_json);
        }
        #endregion Filesystem

        #region HRIS
        public async Task<RestResponse<VezaApiResponse>> PushHRIS(
            string provider_name,
            string data_source_name,
            HRISProvider provider,
            bool save_json = false
        )
        {
            string json_metadata = provider.GetJSONPayload();
            Debug.WriteLine($"Pushing entity payload for provider {provider_name} to data source {data_source_name}");
            return await PushMetadata(provider_name, data_source_name,
                json_metadata, save_json);
        }
        #endregion HRIS

        #region IdP
        public async Task<RestResponse<VezaApiResponse>> PushIdP(
            string provider_name,
            string data_source_name,
            IdPProvider provider,
            bool save_json = false
)
        {
            string json_metadata = provider.GetJSONPayload();
            Debug.WriteLine($"Pushing entity payload for provider {provider_name} to data source {data_source_name}");
            return await PushMetadata(provider_name, data_source_name,
                json_metadata, save_json);
        }
        #endregion

        #region Shared
        /// <summary>
        /// Push an OAA payload to Veza
        /// </summary>
        /// <param name="provider_name">The string name of an existing provider</param>
        /// <param name="data_source_name">The string name of the data_source (will be created if it doesn't exist)</param>
        /// <param name="metadata">The JSON string containing the metadata to be pushed</param>
        /// <param name="save_json">Boolean flag that writes the JSON payload to a file before push. Defaults to false.</param>
        /// <returns>The result of the push metadata POST API call</returns>
        /// <exception cref="OAAClientException">Thrown if the payload exceeds 100MB</exception>
        /// <exception cref="ClientException">Thrown if the provider doesn't exist or if datasource creation fails</exception>
        public async Task<RestResponse<VezaApiResponse>> PushMetadata(
            string provider_name, 
            string data_source_name, 
            string metadata,
            bool save_json = false
        )
        {
            // write json payload to file
            if (save_json)
            {
                string ts = DateTime.Now.ToString("%yyyy%MM%dd-%H%mm%ss");
                string outpath = $"{Directory.GetCurrentDirectory()}\\{data_source_name}_{ts}.json";
                File.WriteAllText(outpath, metadata);
            }

            // ensure provider exists
            string provider_id;
            VezaApiResponse provider = await GetProvider(provider_name);
            if (provider == null)
                throw new ClientException("NO PROVIDER", $"Unable to locate provider {provider_name}; cannot push metadata without existing provider");
            else
                provider_id = provider.values.First()["id"].ToString();

            // ensure data_source exists
            Dictionary<string, object> data_source = await GetDataSource(provider_id: provider_id, data_source_name: data_source_name);
            if (data_source.Count == 0)
            {
                _ = await CreateDataSource(data_source_name: data_source_name, provider_id: provider_id);
                data_source = await GetDataSource(provider_id: provider_id, data_source_name: data_source_name);
            }

            // build base metadata payload
            Dictionary<string, object> payload = new()
            {
                { "id", provider_id },
                { "data_source_id", data_source["id"] }
            };

            // compress payload (if enabled)
            if (configuration.EnableCompression)
            {
                metadata = Sdk.Util.CompressJson(metadata);
                payload.Add("json_data", metadata);
                payload.Add("compression_type", "GZIP");
            }
            else
            {
                payload.Add("json_data", metadata);
            }

            if (Encoding.Unicode.GetByteCount(metadata) > PayloadMaxSize)
                throw new OAAClientException("OVERSIZE", "payload exceeds maximum size of 100MB");

            return await ApiPost($"/api/v1/providers/custom/{provider_id}/datasources/{data_source["id"]}:push", payload);
        }
        #endregion Shared

    }
}