using Microsoft.AspNetCore.Mvc;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    public class ShirtsController : BaseController
    {
        private readonly IWebApiExecuter _webApiExecuter;

        public ShirtsController(
            ILogger<ShirtsController> logger,
            IWebApiExecuter webApiExecuter)
            : base(logger)
        {
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
            catch (Exception ex)
            {
                HandleNetworkException(ex, "fetching shirts from API");
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
                        SetSuccessTempData("Shirt created successfully!");
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (WebApiException ex)
                {
                    _logger.LogError(ex, "API error creating shirt - Status: {StatusCode}", ex.StatusCode);

                    var contextualMessages = new Dictionary<System.Net.HttpStatusCode, string>
                    {
                        { System.Net.HttpStatusCode.Conflict, "A shirt with these properties already exists." }
                    };

                    HandleWebApiException(ex, "An error occurred while creating the shirt. Please try again.", contextualMessages);
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
                var contextualMessages = new Dictionary<System.Net.HttpStatusCode, string>
                {
                    { System.Net.HttpStatusCode.Unauthorized, "You are not authorized to access this shirt." },
                    { System.Net.HttpStatusCode.Forbidden, "Access to this shirt is forbidden." },
                    { System.Net.HttpStatusCode.InternalServerError, "A server error occurred while retrieving the shirt. Please try again later." }
                };
                ViewBag.ErrorMessage = GetWebApiErrorMessage(ex, "An error occurred while retrieving the shirt. Please try again.", contextualMessages);
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

                    var contextualMessages = new Dictionary<System.Net.HttpStatusCode, string>
                    {
                        { System.Net.HttpStatusCode.NotFound, $"Shirt with ID {shirtId} was not found." },
                        { System.Net.HttpStatusCode.Conflict, "A shirt with these properties already exists." }
                    };

                    HandleWebApiException(ex, "An error occurred while updating the shirt. Please try again.", contextualMessages);
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
                SetSuccessTempData("Shirt deleted successfully!");
                return RedirectToAction(nameof(Index));
            }
            catch (WebApiException ex)
            {
                _logger.LogError(ex, "API error deleting shirt with ID {ShirtId} - Status: {StatusCode}", shirtId, ex.StatusCode);

                var contextualMessages = new Dictionary<System.Net.HttpStatusCode, string>
                {
                    { System.Net.HttpStatusCode.NotFound, $"Shirt with ID {shirtId} was not found." },
                    { System.Net.HttpStatusCode.Unauthorized, "You are not authorized to delete this shirt." },
                    { System.Net.HttpStatusCode.Forbidden, "Access to delete this shirt is forbidden." },
                    { System.Net.HttpStatusCode.InternalServerError, "Server error occurred while deleting the shirt. Please try again later." }
                };

                var errorMessage = GetWebApiErrorMessage(ex, "An error occurred while deleting the shirt. Please try again.", contextualMessages);
                SetErrorTempData(errorMessage);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                HandleNetworkException(ex, $"deleting shirt with ID {shirtId}");
                SetErrorTempData(ViewBag.ApiErrorMessage ?? "An unexpected error occurred while deleting the shirt. Please try again.");
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
