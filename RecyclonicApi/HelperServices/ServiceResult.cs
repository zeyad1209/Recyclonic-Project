using Microsoft.AspNetCore.Identity;
using System.Net;

namespace RecyclonicApi.HelperServices
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public int StatusCode { get; set; }
        public static ServiceResult Ok(object? data = null, string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK)
            => new ServiceResult { IsSuccess = true, Data = data, Message = message, StatusCode = (int)statusCode };

        public static ServiceResult Fail(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new ServiceResult { IsSuccess = false, Message = message, StatusCode = (int)statusCode };

        //public static ServiceResult Fail(IEnumerable<IdentityError> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        //    => new ServiceResult { IsSuccess = false, Message = errors.ToString(), StatusCode = (int)statusCode };
    }
}
