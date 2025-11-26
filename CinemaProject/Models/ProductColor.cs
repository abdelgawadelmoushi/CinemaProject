namespace CinemaProject.Models
{
    public class movieColor
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;
        public string Color { get; set; } = string.Empty;
    }
}
