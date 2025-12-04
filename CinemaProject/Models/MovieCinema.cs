namespace CinemaProject.Models
{
    public class MovieCinema
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = new();
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; } = new();
    }
}
