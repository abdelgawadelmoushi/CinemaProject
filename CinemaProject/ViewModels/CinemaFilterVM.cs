namespace CinemaProject.Areas.Customer.Controllers
{
    public class CinemaFilterVM
    {
        public string? CinemaName { get; set; }
        public bool Status { get; set; }
        public int? CategoryId { get; set; }
        public int? CinemaId { get; set; }
        public int Page { get; set; } = 1;
    }
}
