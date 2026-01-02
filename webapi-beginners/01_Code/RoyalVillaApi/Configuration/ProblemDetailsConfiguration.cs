using Microsoft.AspNetCore.Mvc;

namespace RoyalVillaApi.Configuration
{
  public static class ProblemDetailsConfiguration
  {
    public static IServiceCollection AddProblemDetailsSupport(this IServiceCollection services)
    {
      services.AddProblemDetails(options =>
      {
        options.CustomizeProblemDetails = context =>
              {
                // Add additional metadata to all problem details
            context.ProblemDetails.Instance = context.HttpContext.Request.Path;

                // Add trace ID for debugging
            if (context.ProblemDetails.Extensions.ContainsKey("traceId"))
            {
              context.ProblemDetails.Extensions["traceId"] =
                        context.HttpContext.TraceIdentifier;
            }
          };
      });

      // Configure ApiBehaviorOptions to use Problem Details for validation errors
      services.Configure<ApiBehaviorOptions>(options =>
      {
        options.InvalidModelStateResponseFactory = context =>
              {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
              Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
              Title = "One or more validation errors occurred.",
              Status = StatusCodes.Status400BadRequest,
              Instance = context.HttpContext.Request.Path,
              Extensions =
                  {
                            ["traceId"] = context.HttpContext.TraceIdentifier
                  }
            };

            return new BadRequestObjectResult(problemDetails)
            {
              ContentTypes = { "application/problem+json" }
            };
          };
      });

      return services;
    }
  }
}
