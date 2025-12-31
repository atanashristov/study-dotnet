namespace WebApp.Models.Validations
{
  public static class ShirtSizeValidator
  {
    public static bool IsValidSize(string? gender, int? size, out string? errorMessage)
    {
      errorMessage = null;

      if (string.IsNullOrWhiteSpace(gender) || !size.HasValue)
      {
        return true; // No validation needed if either is missing
      }

      if (gender.Equals("Male", StringComparison.OrdinalIgnoreCase) && size < 8)
      {
        errorMessage = "Invalid size for male shirt. Minimum size is 8.";
        return false;
      }

      if (gender.Equals("Female", StringComparison.OrdinalIgnoreCase) && size < 6)
      {
        errorMessage = "Invalid size for female shirt. Minimum size is 6.";
        return false;
      }

      return true;
    }
  }
}
