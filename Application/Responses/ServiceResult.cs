namespace votesystembackend.Application.Responses
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T data, string? message = null) => new ServiceResult<T> { Success = true, StatusCode = 200, Data = data, Message = message };
        public static ServiceResult<T> Created(T data, string? message = null) => new ServiceResult<T> { Success = true, StatusCode = 201, Data = data, Message = message };
        public static ServiceResult<T> Fail(int statusCode, string message) => new ServiceResult<T> { Success = false, StatusCode = statusCode, Message = message };
    }
}
