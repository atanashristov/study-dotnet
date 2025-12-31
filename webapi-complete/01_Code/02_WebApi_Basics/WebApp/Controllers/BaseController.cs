using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApp.Data;

namespace WebApp.Controllers
{
  public abstract class BaseController : Controller
  {
    protected readonly ILogger<BaseController> _logger;

    protected BaseController(ILogger<BaseController> logger)
    {
      _logger = logger;
    }

    /// <summary>
    /// Handles WebApiException by adding specific errors to ModelState or fallback messages
    /// </summary>
    /// <param name="ex">The WebApiException to handle</param>
    /// <param name="defaultMessage">Default message when no specific errors are available</param>
    /// <param name="contextualMessages">Optional contextual messages based on status code</param>
    protected void HandleWebApiException(WebApiException ex, string defaultMessage = "An error occurred. Please try again.", Dictionary<HttpStatusCode, string>? contextualMessages = null)
    {
      // Add specific error messages from the API response
      if (ex.ErrorResponse?.Errors != null)
      {
        foreach (var error in ex.ErrorResponse.Errors)
        {
          foreach (var message in error.Value)
          {
            ModelState.AddModelError(error.Key, message);
          }
        }
      }
      else
      {
        // Check for contextual messages first
        if (contextualMessages?.TryGetValue(ex.StatusCode, out var contextMessage) == true)
        {
          ModelState.AddModelError("", contextMessage);
        }
        else
        {
          // Fallback to generic message based on status code
          var errorMessage = GetStatusCodeErrorMessage(ex.StatusCode, defaultMessage);
          ModelState.AddModelError("", errorMessage);
        }
      }
    }

    /// <summary>
    /// Handles network-related exceptions and sets appropriate ViewBag messages
    /// </summary>
    /// <param name="ex">The exception to handle</param>
    /// <param name="operation">Description of the operation that failed</param>
    protected void HandleNetworkException(Exception ex, string operation)
    {
      switch (ex)
      {
        case System.Net.Sockets.SocketException:
          _logger.LogError(ex, "Network connection error while {Operation}", operation);
          SetApiErrorViewBag("Could not connect to the server. Please check your connection and try again.");
          break;

        case TaskCanceledException when ex.InnerException is TimeoutException:
          _logger.LogError(ex, "Timeout error while {Operation}", operation);
          SetApiErrorViewBag("The request timed out. Please try again.");
          break;

        case HttpRequestException:
          _logger.LogError(ex, "HTTP error while {Operation}", operation);
          SetApiErrorViewBag("Unable to retrieve data from the server. Please try again later.");
          break;

        default:
          _logger.LogError(ex, "Unexpected error while {Operation}", operation);
          SetApiErrorViewBag("An unexpected error occurred. Please try again later.");
          break;
      }
    }

    /// <summary>
    /// Handles WebApiException for ViewBag scenarios (like redirects with TempData)
    /// </summary>
    /// <param name="ex">The WebApiException to handle</param>
    /// <param name="defaultMessage">Default message when no specific handling applies</param>
    /// <param name="contextualMessages">Optional contextual messages based on status code</param>
    /// <returns>User-friendly error message</returns>
    protected string GetWebApiErrorMessage(WebApiException ex, string defaultMessage = "An error occurred. Please try again.", Dictionary<HttpStatusCode, string>? contextualMessages = null)
    {
      // Check for contextual messages first
      if (contextualMessages?.TryGetValue(ex.StatusCode, out var contextMessage) == true)
      {
        return contextMessage;
      }

      // Fallback to status code message or WebApiException's display message
      return ex.StatusCode switch
      {
        HttpStatusCode.NotFound => defaultMessage.Contains("found") ? defaultMessage : "The requested item was not found.",
        HttpStatusCode.Unauthorized => "You are not authorized to perform this action.",
        HttpStatusCode.Forbidden => "Access to this resource is forbidden.",
        HttpStatusCode.InternalServerError => "Server error occurred. Please try again later.",
        _ => ex.GetDisplayMessage()
      };
    }

    /// <summary>
    /// Converts HTTP status codes to user-friendly error messages
    /// </summary>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="defaultMessage">Default message for unhandled status codes</param>
    /// <returns>User-friendly error message</returns>
    protected string GetStatusCodeErrorMessage(HttpStatusCode statusCode, string defaultMessage = "An error occurred. Please try again.")
    {
      return statusCode switch
      {
        HttpStatusCode.BadRequest => "Invalid data provided. Please check your input and try again.",
        HttpStatusCode.Unauthorized => "You are not authorized to perform this action.",
        HttpStatusCode.Forbidden => "Access to this resource is forbidden.",
        HttpStatusCode.NotFound => "The requested item was not found.",
        HttpStatusCode.Conflict => "A conflict occurred with existing data.",
        HttpStatusCode.UnprocessableEntity => "The request was well-formed but contains semantic errors.",
        HttpStatusCode.InternalServerError => "Server error occurred. Please try again later.",
        _ => defaultMessage
      };
    }

    /// <summary>
    /// Sets consistent API error flags in ViewBag
    /// </summary>
    /// <param name="errorMessage">The error message to display</param>
    protected void SetApiErrorViewBag(string errorMessage)
    {
      ViewBag.ApiError = true;
      ViewBag.ApiErrorMessage = errorMessage;
    }

    /// <summary>
    /// Sets error message in TempData for display after redirect
    /// </summary>
    /// <param name="errorMessage">The error message to display</param>
    protected void SetErrorTempData(string errorMessage)
    {
      TempData["ErrorMessage"] = errorMessage;
    }

    /// <summary>
    /// Sets success message in TempData for display after redirect
    /// </summary>
    /// <param name="successMessage">The success message to display</param>
    protected void SetSuccessTempData(string successMessage)
    {
      TempData["SuccessMessage"] = successMessage;
    }
  }
}
