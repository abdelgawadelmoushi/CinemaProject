namespace CinemaProject.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Img { get; set; } = string.Empty;

        public List<string> Skills { get; set; } = new List<string>();
        public ICollection<ActorMovie> ActorMovies { get; set; } = new List<ActorMovie>();

        public ICollection<ActorCategory> ActorCategories { get; set; } = new List<ActorCategory>();
        public ICollection<ActorCinema> ActorCinema { get; set; } = new List<ActorCinema>();
    }
}
