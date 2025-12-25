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
                catch (WebApiException ex)
                {
                    _logger.LogError(ex, "API error creating shirt - Status: {StatusCode}", ex.StatusCode);

                    // Add specific error messages from the API response
                    if (ex.ErrorResponse?.Errors != null)
                    {
                        foreach (var error in ex.ErrorResponse.Errors)
                        {
                            foreach (var message in error.Value)
                            {
                                ModelState.AddModelError("", message);
                            }
                        }
                    }
                    else
                    {
                        // Fallback to generic message based on status code
                        var errorMessage = ex.StatusCode switch
                        {
                            System.Net.HttpStatusCode.BadRequest => "Invalid data provided. Please check your input and try again.",
                            System.Net.HttpStatusCode.Conflict => "A shirt with these properties already exists.",
                            System.Net.HttpStatusCode.InternalServerError => "Server error occurred. Please try again later.",
                            System.Net.HttpStatusCode.Unauthorized => "You are not authorized to perform this action.",
                            System.Net.HttpStatusCode.Forbidden => "Access to this resource is forbidden.",
                            System.Net.HttpStatusCode.UnprocessableEntity => "The request was well-formed but contains semantic errors.",
                            _ => "An error occurred while creating the shirt. Please try again."
                        };
                        ModelState.AddModelError("", errorMessage);
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
            catch (WebApiException ex)
            {
                _logger.LogWarning(ex, "API error retrieving shirt with ID {ShirtId} - Status: {StatusCode}", shirtId, ex.StatusCode);

                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.ShirtId = shirtId;
                    ViewBag.ErrorMessage = $"Shirt with ID {shirtId} does not exist.";
                    return View("ShirtNotFound");
                }

                // For other HTTP errors, show a generic error
                ViewBag.ShirtId = shirtId;
                ViewBag.ErrorMessage = ex.StatusCode switch
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
                catch (WebApiException ex)
                {
                    _logger.LogError(ex, "API error updating shirt with ID {ShirtId} - Status: {StatusCode}", shirtId, ex.StatusCode);

                    // Add specific error messages from the API response
                    if (ex.ErrorResponse?.Errors != null)
                    {
                        foreach (var error in ex.ErrorResponse.Errors)
                        {
                            foreach (var message in error.Value)
                            {
                                ModelState.AddModelError("", message);
                            }
                        }
                    }
                    else
                    {
                        // Fallback to generic message based on status code
                        var errorMessage = ex.StatusCode switch
                        {
                            System.Net.HttpStatusCode.BadRequest => "Invalid data provided. Please check your input and try again.",
                            System.Net.HttpStatusCode.NotFound => $"Shirt with ID {shirtId} was not found.",
                            System.Net.HttpStatusCode.Conflict => "A shirt with these properties already exists.",
                            System.Net.HttpStatusCode.InternalServerError => "Server error occurred. Please try again later.",
                            System.Net.HttpStatusCode.Unauthorized => "You are not authorized to perform this action.",
                            System.Net.HttpStatusCode.Forbidden => "Access to this resource is forbidden.",
                            System.Net.HttpStatusCode.UnprocessableEntity => "The request was well-formed but contains semantic errors.",
                            _ => "An error occurred while updating the shirt. Please try again."
                        };
                        ModelState.AddModelError("", errorMessage);
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
            catch (WebApiException ex)
            {
                _logger.LogError(ex, "API error deleting shirt with ID {ShirtId} - Status: {StatusCode}", shirtId, ex.StatusCode);

                var errorMessage = ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => $"Shirt with ID {shirtId} was not found.",
                    System.Net.HttpStatusCode.Unauthorized => "You are not authorized to delete this shirt.",
                    System.Net.HttpStatusCode.Forbidden => "Access to delete this shirt is forbidden.",
                    System.Net.HttpStatusCode.InternalServerError => "Server error occurred while deleting the shirt. Please try again later.",
                    _ => ex.GetDisplayMessage()
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


        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }

    }


}
