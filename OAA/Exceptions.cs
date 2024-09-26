using Veza.Sdk.Exceptions;

namespace Veza.OAA.Exceptions
{
    /// <summary>
    /// An exception class that inherits from the base Veza ClientException
    /// 
    /// This has no additional functionality, but helps to separate concerns
    /// </summary>
    public class OAAClientException : ClientException
    {
        public OAAClientException(string error, string message, int? status_code = null) 
            : base(error, message, status_code) {}

        public OAAClientException(string error, string message, Exception inner_exception, int? status_code = null)
            : base(error, message, inner_exception, status_code) { }


        public OAAClientException(string error, string message, List<string> details, int? status_code = null)
            : base(error, message, details, status_code) { }


        public OAAClientException(string error, string message, Exception inner_exception, List<string> details, int? status_code = null)
            : base(error, message, inner_exception, details, status_code) { }
    }

    /// <summary>
    /// General exception for violations of the OAA template schema 
    /// </summary>
    public class TemplateException : VezaException
    {
        public TemplateException(string message) : base(message)
        { }

        public TemplateException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
