using System.Net;

namespace DemoAPI.ApplicationModel
{
    public class APIResponse
    {
        public HttpStatusCode statusCode { get; set; }

        public bool IsSuccess = false;

        public object result { get; set; }

        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
