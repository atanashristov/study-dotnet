using Microsoft.AspNetCore.Mvc;
using RoyalVillaApi.Helpers;

namespace RoyalVillaApi.Controllers
{
  [ApiController]
  public abstract class BaseApiController : ControllerBase
  {
    /// <summary>
    /// Creates a BadRequest (400) problem details response
    /// </summary>
    protected ObjectResult ProblemBadRequest(string detail)
    {
      var problemDetails = ProblemDetailsHelper.CreateBadRequest(detail, Request.Path);
      return new ObjectResult(problemDetails)
      {
        StatusCode = problemDetails.Status
      };
    }

    /// <summary>
    /// Creates a NotFound (404) problem details response
    /// </summary>
    protected ObjectResult ProblemNotFound(string detail)
    {
      var problemDetails = ProblemDetailsHelper.CreateNotFound(detail, Request.Path);
      return new ObjectResult(problemDetails)
      {
        StatusCode = problemDetails.Status
      };
    }

    /// <summary>
    /// Creates an InternalServerError (500) problem details response
    /// </summary>
    protected ObjectResult ProblemInternalServerError(string detail = "An error occurred while processing your request.")
    {
      var problemDetails = ProblemDetailsHelper.CreateInternalServerError(detail, Request.Path);
      return new ObjectResult(problemDetails)
      {
        StatusCode = problemDetails.Status
      };
    }

    /// <summary>
    /// Creates an Unauthorized (401) problem details response
    /// </summary>
    protected ObjectResult ProblemUnauthorized(string detail = "Authentication is required.")
    {
      var problemDetails = ProblemDetailsHelper.CreateUnauthorized(detail, Request.Path);
      return new ObjectResult(problemDetails)
      {
        StatusCode = problemDetails.Status
      };
    }

    /// <summary>
    /// Creates a Forbidden (403) problem details response
    /// </summary>
    protected ObjectResult ProblemForbidden(string detail = "You do not have permission to access this resource.")
    {
      var problemDetails = ProblemDetailsHelper.CreateForbidden(detail, Request.Path);
      return new ObjectResult(problemDetails)
      {
        StatusCode = problemDetails.Status
      };
    }

    /// <summary>
    /// Creates a Conflict (409) problem details response
    /// </summary>
    protected ObjectResult ProblemConflict(string detail)
    {
      var problemDetails = ProblemDetailsHelper.CreateConflict(detail, Request.Path);
      return new ObjectResult(problemDetails)
      {
        StatusCode = problemDetails.Status
      };
    }

    /// <summary>
    /// Creates a custom problem details response
    /// </summary>
    protected ObjectResult ProblemCustom(int statusCode, string title, string detail, string type)
    {
      var problemDetails = ProblemDetailsHelper.CreateCustom(statusCode, title, detail, type, Request.Path);
      return new ObjectResult(problemDetails)
      {
        StatusCode = problemDetails.Status
      };
    }
  }
}
