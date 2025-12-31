namespace WebApp.Models.Repositories
{
  public static class ShirtRepository
  {
    private static List<Shirt> shirts = new List<Shirt>
            {
                new Shirt { ShirtId = 1, Brand = "Nike", Color = "Red", Size = 10, Gender = "Male", Price = 29.99 },
                new Shirt { ShirtId = 2, Brand = "Adidas", Color = "Blue", Size = 12, Gender = "Male", Price = 34.99 },
                new Shirt { ShirtId = 3, Brand = "Puma", Color = "Green", Size = 8, Gender = "Female", Price = 24.99 }
            };

    public static List<Shirt> GetAllShirts()
    {
      return shirts;
    }

    public static bool ShirtExists(int id)
    {
      return shirts.Any(s => s.ShirtId == id);
    }

    public static Shirt? GetShirtById(int id)
    {
      return shirts.FirstOrDefault(s => s.ShirtId == id);
    }

    public static Shirt? GetShirtByProperties(
        string brand,
        string color,
        string gender,
        int? size)
    {
      return shirts.FirstOrDefault(s =>
          !string.IsNullOrWhiteSpace(brand) &&
          !string.IsNullOrWhiteSpace(s.Brand) &&
          s.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase) &&
          !string.IsNullOrWhiteSpace(color) &&
          !string.IsNullOrWhiteSpace(s.Color) &&
          s.Color.Equals(color, StringComparison.OrdinalIgnoreCase) &&
          !string.IsNullOrWhiteSpace(gender) &&
          !string.IsNullOrWhiteSpace(s.Gender) &&
          s.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase) &&
          size.HasValue &&
          s.Size.HasValue &&
          s.Size == size);
    }

    public static Shirt AddShirt(Shirt shirt)
    {
      if (shirt.ShirtId >= 0 && shirts.Any(s => s.ShirtId == shirt.ShirtId))
      {
        throw new ArgumentException($"A shirt with ID {shirt.ShirtId} already exists.");
      }

      if (shirt.ShirtId <= 0)
      {
        shirt.ShirtId = shirts.Max(s => s.ShirtId) + 1;
      }
      shirts.Add(shirt);

      return shirt;
    }

    public static void UpdateShirt(Shirt updatedShirt)
    {
      if (updatedShirt.ShirtId <= 0)
      {
        throw new ArgumentException("Shirt ID must be a positive integer.");
      }

      var index = shirts.FindIndex(s => s.ShirtId == updatedShirt.ShirtId);
      if (index == -1)
      {
        throw new ArgumentException($"Shirt with ID {updatedShirt.ShirtId} not found.");
      }

      shirts[index] = updatedShirt;
    }

    public static void DeleteShirt(int id)
    {
      var shirt = shirts.FirstOrDefault(s => s.ShirtId == id);
      if (shirt != null)
      {
        shirts.Remove(shirt);
      }
    }
  }
}
