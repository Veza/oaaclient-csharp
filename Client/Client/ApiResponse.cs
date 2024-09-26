namespace Veza.Sdk.Client
{
    public interface IApiResponse
    {
        Type ResponseType { get; }
    }

    public class ApiResponse<T> : IApiResponse
    {
        public T? Data { get; }
        public Type ResponseType { get { return typeof(T); } }

        public ApiResponse(T data)
        { 
            Data = data;
        }

        public ApiResponse()
        {

        }
    }
}
