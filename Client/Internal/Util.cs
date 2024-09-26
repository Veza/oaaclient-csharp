using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using Veza.Sdk.Exceptions;

namespace Veza.Sdk
{
    public static class Util
    {
        /// <summary>
        /// Gzip Compress a JSON payload for sending to a Veza instance
        /// </summary>
        /// <param name="json">The string json to be compressed</param>
        /// <returns>The base64-encoded gzipped json payload</returns>
        public static string CompressJson(string json)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            var memory_stream = new MemoryStream();
            using (var gzip_stream = new GZipStream(memory_stream, CompressionMode.Compress, true))
            {
                gzip_stream.Write(buffer, 0, buffer.Length);
            }

            return Convert.ToBase64String(memory_stream.ToArray());
        }

        /// <summary>
        /// Parse an incoming URL string and ensure it is properly formatted for the HttpClient
        /// </summary>
        /// <param name="client_url">The string URL to parse</param>
        /// <returns>
        /// The string URL formatted for use with HttpClient
        /// </returns>
        public static string ParseURL(string client_url)
        {
            if (string.IsNullOrWhiteSpace(client_url))
            {
                throw new ClientException("URL validation failed", "URL provided is empty");
            }

            Match m = Regex.Match(client_url, @"^https?:\/\/.*", RegexOptions.IgnoreCase);
            Match trailing_slash = Regex.Match(client_url, @"\/$");
            if (m.Success)
            {
                if (trailing_slash.Success) { return client_url; }
                else { return $"{client_url}/"; }
            }
            else
            {
                if (trailing_slash.Success) { return $"https://{client_url}"; }
                else { return $"https://{client_url}/"; }
            }
        }

        public static RestSharp.Method ToRestSharp(this HttpMethod method)
        {
            if (method == HttpMethod.Get) return RestSharp.Method.Get;
            if (method == HttpMethod.Head) return RestSharp.Method.Head;
            if (method == HttpMethod.Delete) return RestSharp.Method.Delete;
            if (method == HttpMethod.Put) return RestSharp.Method.Put;
            if (method == HttpMethod.Post) return RestSharp.Method.Post;
            if (method == HttpMethod.Patch) return RestSharp.Method.Patch;
            if (method == HttpMethod.Options) return RestSharp.Method.Options;

            throw new ArgumentException($"Invalid HTTP Method provided: {method}");
        }
    }
}
