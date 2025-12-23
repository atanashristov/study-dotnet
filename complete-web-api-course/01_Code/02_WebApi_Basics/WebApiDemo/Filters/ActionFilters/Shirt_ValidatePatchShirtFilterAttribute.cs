using Microsoft.AspNetCore.Mvc.Filters;
using WebApiDemo.Models;
using WebApiDemo.Models.Repositories;
using WebApiDemo.Models.Validations;
using WebApiDemo.Utilities;

namespace WebApiDemo.Filters
{
  public class Shirt_ValidatePatchShirtFilterAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      base.OnActionExecuting(context);

      // Validate that we have a valid DTO
      context.ActionArguments.TryGetValue("partialUpdateShirtDto", out var partialUpdateShirtDtoObj);
      if (partialUpdateShirtDtoObj == null || partialUpdateShirtDtoObj is not PartialUpdateShirtDto)
      {
        ValidationProblemDetailsHelper.SetValidationErrorResult(
            context,
            "partialUpdateShirtDto",
            "Invalid request body.");
        return;
      }

      var shirtId = context.ActionArguments["id"] as int? ?? 0;
      var partialUpdateShirtDto = (PartialUpdateShirtDto)partialUpdateShirtDtoObj;

      // Get the existing shirt for validation
      var existingShirt = ShirtRepository.GetShirtById(shirtId);
      if (existingShirt == null)
      {
        ValidationProblemDetailsHelper.SetValidationErrorResult(
            context,
            "id",
            "Shirt not found.",
            StatusCodes.Status404NotFound);
        return;
      }

      // Validate size/gender combination if either is being updated
      if (partialUpdateShirtDto.Size.HasValue || partialUpdateShirtDto.Gender != null)
      {
        var gender = partialUpdateShirtDto.Gender ?? existingShirt.Gender;
        var size = partialUpdateShirtDto.Size ?? existingShirt.Size;

        if (!ShirtSizeValidator.IsValidSize(gender, size, out var errorMessage))
        {
          ValidationProblemDetailsHelper.SetValidationErrorResult(
              context,
              "size",
              errorMessage ?? "Invalid size for the selected gender.");
          return;
        }
      }

      // Check for duplicate shirt (only if we're updating properties that would create a duplicate)
      if (partialUpdateShirtDto.Brand != null || partialUpdateShirtDto.Color != null ||
          partialUpdateShirtDto.Gender != null || partialUpdateShirtDto.Size.HasValue)
      {
        var brand = partialUpdateShirtDto.Brand ?? existingShirt.Brand;
        var color = partialUpdateShirtDto.Color ?? existingShirt.Color;
        var gender = partialUpdateShirtDto.Gender ?? existingShirt.Gender;
        var size = partialUpdateShirtDto.Size ?? existingShirt.Size;

        var existingShirtWithSameProperties = ShirtRepository.GetShirtByProperties(brand, color, gender, size);

        if (existingShirtWithSameProperties != null && existingShirtWithSameProperties.ShirtId != shirtId)
        {
          ValidationProblemDetailsHelper.SetValidationErrorResult(
              context,
              "partialUpdateShirtDto",
              "A shirt with the same brand, color, size, and gender already exists.");
          return;
        }
      }
    }
  }
}
