namespace CinemaProject.ViewModels
{
    public record movieFilterVM(string? MovieName, decimal? minPrice, decimal? maxPrice, bool lessQuantity, bool status, int? categoryId, int? CinemaId, int page = 1);
}
