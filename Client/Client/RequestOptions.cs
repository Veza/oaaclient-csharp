namespace Veza.Sdk.Client
{
    public class RequestOptions(object? data = null, Dictionary<string, Stream>? file_data = null, Dictionary<string, string>? headers = null,
        string? operation_type = null, Dictionary<string, string>? path_params = null, Dictionary<string, string>? query_params = null)
    {
        public object? Data { get; set; } = data;
        public Dictionary<string, Stream> FileData { get; set; } = file_data ?? [];
        public Dictionary<string, string> Headers { get; set; } = headers ?? [];
        public string? OperationType { get; set; } = operation_type;
        public Dictionary<string, string> PathParams { get; set; } = path_params ?? [];
        public Dictionary<string, string> QueryParams { get; set; } = query_params ?? [];
    }
}
