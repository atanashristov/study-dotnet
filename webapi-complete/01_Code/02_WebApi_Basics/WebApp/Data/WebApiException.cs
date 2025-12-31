using System.Net;
using System.Text.Json;

namespace WebApp.Data
{
    public class WebApiException : HttpRequestException
    {
        public ErrorResponse? ErrorResponse { get; set; }
        public new HttpStatusCode StatusCode { get; set; }
        public string? ReasonPhrase { get; set; }

        public WebApiException(string message, string errorJson, HttpStatusCode statusCode, string? reasonPhrase)
            : base(message)
        {
            try
            {
                ErrorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorJson);
            }
            catch (JsonException)
            {
                // If JSON deserialization fails, ErrorResponse will remain null
                // but we still have the raw errorJson in the message
            }

            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
        }

        /// <summary>
        /// Gets a user-friendly error message combining HTTP status info and error details.
        /// </summary>
        public string GetDisplayMessage()
        {
            if (ErrorResponse != null)
            {
                return ErrorResponse.GetDisplayMessage();
            }

            return $"HTTP {(int)StatusCode} {ReasonPhrase}: {Message}";
        }

        /// <summary>
        /// Determines if this is a client error (4xx status code).
        /// </summary>
        public bool IsClientError => (int)StatusCode >= 400 && (int)StatusCode < 500;

        /// <summary>
        /// Determines if this is a server error (5xx status code).
        /// </summary>
        public bool IsServerError => (int)StatusCode >= 500;
    }
}
