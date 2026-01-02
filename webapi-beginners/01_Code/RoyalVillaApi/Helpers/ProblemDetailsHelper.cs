using Microsoft.AspNetCore.Mvc;

namespace RoyalVillaApi.Helpers
{
  /// <summary>
  /// Static helper methods for creating RFC 9110 compliant ProblemDetails responses
  /// </summary>
  public static class ProblemDetailsHelper
  {
    /// <summary>
    /// Creates a BadRequest (400) ProblemDetails object
    /// </summary>
    public static ProblemDetails CreateBadRequest(string detail, string? instance = null)
    {
      return new ProblemDetails
      {
        Status = StatusCodes.Status400BadRequest,
        Title = "Bad Request",
        Detail = detail,
        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        Instance = instance
      };
    }

    /// <summary>
    /// Creates a NotFound (404) ProblemDetails object
    /// </summary>
    public static ProblemDetails CreateNotFound(string detail, string? instance = null)
    {
      return new ProblemDetails
      {
        Status = StatusCodes.Status404NotFound,
        Title = "Not Found",
        Detail = detail,
        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        Instance = instance
      };
    }

    /// <summary>
    /// Creates an InternalServerError (500) ProblemDetails object
    /// </summary>
    public static ProblemDetails CreateInternalServerError(
        string detail = "An error occurred while processing your request.",
        string? instance = null)
    {
      return new ProblemDetails
      {
        Status = StatusCodes.Status500InternalServerError,
        Title = "Internal Server Error",
        Detail = detail,
        Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
        Instance = instance
      };
    }

    /// <summary>
    /// Creates a Unauthorized (401) ProblemDetails object
    /// </summary>
    public static ProblemDetails CreateUnauthorized(
        string detail = "Authentication is required.",
        string? instance = null)
    {
      return new ProblemDetails
      {
        Status = StatusCodes.Status401Unauthorized,
        Title = "Unauthorized",
        Detail = detail,
        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        Instance = instance
      };
    }

    /// <summary>
    /// Creates a Forbidden (403) ProblemDetails object
    /// </summary>
    public static ProblemDetails CreateForbidden(
        string detail = "You do not have permission to access this resource.",
        string? instance = null)
    {
      return new ProblemDetails
      {
        Status = StatusCodes.Status403Forbidden,
        Title = "Forbidden",
        Detail = detail,
        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        Instance = instance
      };
    }

    /// <summary>
    /// Creates a Conflict (409) ProblemDetails object
    /// </summary>
    public static ProblemDetails CreateConflict(string detail, string? instance = null)
    {
      return new ProblemDetails
      {
        Status = StatusCodes.Status409Conflict,
        Title = "Conflict",
        Detail = detail,
        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        Instance = instance
      };
    }

    /// <summary>
    /// Creates a custom ProblemDetails object
    /// </summary>
    public static ProblemDetails CreateCustom(
        int statusCode,
        string title,
        string detail,
        string type,
        string? instance = null)
    {
      return new ProblemDetails
      {
        Status = statusCode,
        Title = title,
        Detail = detail,
        Type = type,
        Instance = instance
      };
    }
  }
}
