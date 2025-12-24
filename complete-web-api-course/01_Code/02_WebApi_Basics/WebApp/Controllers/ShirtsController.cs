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
            try
            {
                var shirts = await _webApiExecuter.InvokeGetAsync<List<Shirt>>("shirts");
                return View(shirts);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                _logger.LogError(ex, "Network connection error while fetching shirts from API");
                ViewBag.ApiError = true;
                ViewBag.ApiErrorMessage = "Could not connect to the server. Please check your connection and try again.";
                return View(new List<Shirt>());
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while fetching shirts from API");
                ViewBag.ApiError = true;
                ViewBag.ApiErrorMessage = "Unable to retrieve shirts from the server. Please try again later.";
                return View(new List<Shirt>());
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError(ex, "Timeout error while fetching shirts from API");
                ViewBag.ApiError = true;
                ViewBag.ApiErrorMessage = "The request timed out. Please try again.";
                return View(new List<Shirt>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching shirts from API");
                ViewBag.ApiError = true;
                ViewBag.ApiErrorMessage = "An unexpected error occurred. Please try again later.";
                return View(new List<Shirt>());
            }
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


        [HttpGet("Update/{shirtId}")]
        public async Task<IActionResult> UpdateShirt(int shirtId)
        {
            try
            {
                var shirtToUpdate = await _webApiExecuter.InvokeGetAsync<Shirt>($"shirts/{shirtId}");
                if (shirtToUpdate != null)
                {
                    return View(UpdateShirtDto.FromEntity(shirtToUpdate));
                }

                // This shouldn't happen if the API is working correctly, but handle it just in case
                ViewBag.ShirtId = shirtId;
                ViewBag.ErrorMessage = "The requested shirt was not found.";
                return View("ShirtNotFound");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Error retrieving shirt with ID {ShirtId} from API", shirtId);

                var statusCode = GetHttpStatusCodeFromException(ex);

                if (statusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.ShirtId = shirtId;
                    ViewBag.ErrorMessage = $"Shirt with ID {shirtId} does not exist.";
                    return View("ShirtNotFound");
                }

                // For other HTTP errors, show a generic error
                ViewBag.ShirtId = shirtId;
                ViewBag.ErrorMessage = statusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => "You are not authorized to access this shirt.",
                    System.Net.HttpStatusCode.Forbidden => "Access to this shirt is forbidden.",
                    System.Net.HttpStatusCode.InternalServerError => "A server error occurred while retrieving the shirt. Please try again later.",
                    _ => "An error occurred while retrieving the shirt. Please try again."
                };
                return View("ShirtNotFound");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving shirt with ID {ShirtId}", shirtId);
                ViewBag.ShirtId = shirtId;
                ViewBag.ErrorMessage = "An unexpected error occurred while retrieving the shirt.";
                return View("ShirtNotFound");
            }
        }

        [HttpPost("Update/{shirtId}")]
        public async Task<IActionResult> UpdateShirt(int shirtId, UpdateShirtDto updateShirtDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _webApiExecuter.InvokePutAsync<UpdateShirtDto, Shirt>($"shirts/{shirtId}", updateShirtDto);

                    ViewBag.SuccessMessage = "Shirt updated successfully!";
                    return View(updateShirtDto);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error updating shirt with ID {ShirtId} via API", shirtId);

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
                            case System.Net.HttpStatusCode.NotFound:
                                ModelState.AddModelError("", $"Shirt with ID {shirtId} was not found.");
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
                                ModelState.AddModelError("", "An error occurred while updating the shirt. Please try again.");
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error updating shirt");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            var shirtToUpdate = await _webApiExecuter.InvokeGetAsync<Shirt>($"shirts/{shirtId}");
            if (shirtToUpdate != null)
            {
                return View(UpdateShirtDto.FromEntity(shirtToUpdate));
            }

            // This shouldn't happen if the API is working correctly, but handle it just in case
            ViewBag.ShirtId = shirtId;
            ViewBag.ErrorMessage = "The requested shirt was not found.";
            return View("ShirtNotFound");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteShirt(int shirtId)
        {
            try
            {
                await _webApiExecuter.InvokeDeleteAsync($"shirts/{shirtId}");
                TempData["SuccessMessage"] = "Shirt deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error deleting shirt with ID {ShirtId} via API", shirtId);

                var statusCode = GetHttpStatusCodeFromException(ex);

                var errorMessage = statusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => $"Shirt with ID {shirtId} was not found.",
                    System.Net.HttpStatusCode.Unauthorized => "You are not authorized to delete this shirt.",
                    System.Net.HttpStatusCode.Forbidden => "Access to delete this shirt is forbidden.",
                    System.Net.HttpStatusCode.InternalServerError => "Server error occurred while deleting the shirt. Please try again later.",
                    _ => "An error occurred while deleting the shirt. Please try again."
                };

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(Index));
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                _logger.LogError(ex, "Network connection error while deleting shirt with ID {ShirtId}", shirtId);
                TempData["ErrorMessage"] = "Could not connect to the server. Please check your connection and try again.";
                return RedirectToAction(nameof(Index));
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError(ex, "Timeout error while deleting shirt with ID {ShirtId}", shirtId);
                TempData["ErrorMessage"] = "The request timed out. Please try again.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting shirt with ID {ShirtId}", shirtId);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the shirt. Please try again.";
                return RedirectToAction(nameof(Index));
            }
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
