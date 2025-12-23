using Microsoft.AspNetCore.Mvc;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.Repositories;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    public class ShirtsController : Controller
    {
        private readonly ILogger<ShirtsController> _logger;
        private readonly IWebApiExecuter _webApiExecuter;

        public ShirtsController(
            ILogger<ShirtsController> logger,
            IWebApiExecuter webApiExecuter)
        {
            _logger = logger;
            _webApiExecuter = webApiExecuter;
        }

        [HttpGet]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _webApiExecuter.InvokeGetAsync<List<Shirt>>("shirts"));
        }

        [HttpGet("Create")]
        public async Task<IActionResult> CreateShirt()
        {
            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateShirt(CreateShirtDto createShirtDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _webApiExecuter.InvokePostAsync<CreateShirtDto, Shirt>("shirts", createShirtDto);

                    if (response != null)
                    {
                        TempData["SuccessMessage"] = "Shirt created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error creating shirt via API");

                    // Try to extract detailed error messages from the response
                    bool errorAdded = false;

                    try
                    {
                        // Check if the exception data contains response content
                        if (ex.Data.Contains("ResponseContent"))
                        {
                            var responseContent = ex.Data["ResponseContent"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(responseContent))
                            {
                                var problemDetails = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

                                if (problemDetails?.ContainsKey("errors") == true)
                                {
                                    var errorsJson = problemDetails["errors"].ToString();
                                    if (!string.IsNullOrWhiteSpace(errorsJson))
                                    {
                                        var errors = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string[]>>(errorsJson)
                                            ?? Enumerable.Empty<KeyValuePair<string, string[]>>();

                                        foreach (var error in errors)
                                        {
                                            foreach (var message in error.Value)
                                            {
                                                ModelState.AddModelError("", message);
                                                errorAdded = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception parseEx)
                    {
                        _logger.LogWarning(parseEx, "Could not parse error response from API");
                    }

                    // If we couldn't extract specific errors, use generic messages based on status code
                    if (!errorAdded)
                    {
                        // Try to get the actual HTTP status code from the exception
                        var statusCode = GetHttpStatusCodeFromException(ex);

                        switch (statusCode)
                        {
                            case System.Net.HttpStatusCode.BadRequest:
                                ModelState.AddModelError("", "Invalid data provided. Please check your input and try again.");
                                break;
                            case System.Net.HttpStatusCode.Conflict:
                                ModelState.AddModelError("", "A shirt with these properties already exists.");
                                break;
                            case System.Net.HttpStatusCode.InternalServerError:
                                ModelState.AddModelError("", "Server error occurred. Please try again later.");
                                break;
                            case System.Net.HttpStatusCode.Unauthorized:
                                ModelState.AddModelError("", "You are not authorized to perform this action.");
                                break;
                            case System.Net.HttpStatusCode.Forbidden:
                                ModelState.AddModelError("", "Access to this resource is forbidden.");
                                break;
                            case System.Net.HttpStatusCode.UnprocessableEntity:
                                ModelState.AddModelError("", "The request was well-formed but contains semantic errors.");
                                break;
                            default:
                                ModelState.AddModelError("", "An error occurred while creating the shirt. Please try again.");
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error creating shirt");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            return View(createShirtDto);
        }

        private static System.Net.HttpStatusCode? GetHttpStatusCodeFromException(HttpRequestException ex)
        {
            // For .NET 5+ versions, HttpRequestException has a Data property that might contain status code
            if (ex.Data.Contains("StatusCode"))
            {
                if (ex.Data["StatusCode"] is System.Net.HttpStatusCode statusCode)
                {
                    return statusCode;
                }
                if (int.TryParse(ex.Data["StatusCode"]?.ToString(), out var statusCodeInt))
                {
                    return (System.Net.HttpStatusCode)statusCodeInt;
                }
            }

            // Alternative: Try to extract from HttpRequestError (if available)
            if (ex.Data.Contains("HttpRequestError"))
            {
                // This would be for newer .NET versions with HttpRequestError
                return ex.Data["HttpRequestError"] switch
                {
                    _ when ex.Data["HttpRequestError"]?.ToString()?.Contains("BadRequest") == true => System.Net.HttpStatusCode.BadRequest,
                    _ when ex.Data["HttpRequestError"]?.ToString()?.Contains("Unauthorized") == true => System.Net.HttpStatusCode.Unauthorized,
                    _ when ex.Data["HttpRequestError"]?.ToString()?.Contains("Forbidden") == true => System.Net.HttpStatusCode.Forbidden,
                    _ when ex.Data["HttpRequestError"]?.ToString()?.Contains("NotFound") == true => System.Net.HttpStatusCode.NotFound,
                    _ when ex.Data["HttpRequestError"]?.ToString()?.Contains("Conflict") == true => System.Net.HttpStatusCode.Conflict,
                    _ when ex.Data["HttpRequestError"]?.ToString()?.Contains("InternalServerError") == true => System.Net.HttpStatusCode.InternalServerError,
                    _ => null
                };
            }

            // Fallback: Parse from exception message (less reliable but sometimes necessary)
            var message = ex.Message;
            if (message.Contains("400")) return System.Net.HttpStatusCode.BadRequest;
            if (message.Contains("401")) return System.Net.HttpStatusCode.Unauthorized;
            if (message.Contains("403")) return System.Net.HttpStatusCode.Forbidden;
            if (message.Contains("404")) return System.Net.HttpStatusCode.NotFound;
            if (message.Contains("409")) return System.Net.HttpStatusCode.Conflict;
            if (message.Contains("422")) return System.Net.HttpStatusCode.UnprocessableEntity;
            if (message.Contains("500")) return System.Net.HttpStatusCode.InternalServerError;

            return null; // Unknown status code
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}
