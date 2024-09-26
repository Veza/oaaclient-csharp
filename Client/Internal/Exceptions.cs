namespace Veza.Sdk.Exceptions
{
    public class VezaException : Exception
    {
        public string Error { get; set; }
        public List<string> Details { get; set; }

        public VezaException(string error) : base(error)
        {
            Details = [];
            Error = error;
        }
        public VezaException(string error, Exception inner_exception) : base(error, inner_exception)
        {
            Details = [];
            Error = error;
        }
        public VezaException(string error, string message) : base(message)
        {
            Details = [];
            Error = error;
        }
        public VezaException(string error, string message, Exception inner_exception) : base(message, inner_exception)
        {
            Details = [];
            Error = error;
        }
        public VezaException(string error, string message, List<string> details) : base(message)
        {
            Details = details;
            Error = error;
        }
        public VezaException(string error, string message, List<string> details, Exception inner_exception) : base(message, inner_exception)
        {
            Details = details;
            Error = error;
        }
    }

    public class ClientException : VezaException
    {
        public int? StatusCode { get; set; }

        public ClientException(string error, string message, int? status_code = null) : base(error, message)
        {
            StatusCode = status_code;
        }

        public ClientException(string error, string message, Exception inner_exception, int? status_code = null) : base(error, message, inner_exception)
        {
            StatusCode = status_code;
        }

        public ClientException(string error, string message, List<string> details, int? status_code = null) : base(error, message, details)
        {
            StatusCode = status_code;
        }

        public ClientException(string error, string message, Exception inner_exception, List<string> details, int? status_code = null) : base(error, message, details, inner_exception)
        {
            StatusCode = status_code;
        }
    }

    public class ResponseException : VezaException
    {
        public string RequestID { get; set; }
        public DateTime Timestamp { get; set; }

        public ResponseException(string request_id, DateTime timestamp) : base("A Veza client request exception occurred")
        {
            RequestID = request_id;
            Timestamp = timestamp;
        }
        public ResponseException(string request_id, DateTime timestamp, Exception inner_exception) : base("A Veza client request exception occurred", inner_exception)
        {
            RequestID = request_id;
            Timestamp = timestamp;
        }
        public ResponseException(string error, string message, string request_id, DateTime timestamp) : base(error, message)
        {
            RequestID = request_id;
            Timestamp = timestamp;
        }
    }
}
