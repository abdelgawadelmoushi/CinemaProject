using System.ComponentModel.DataAnnotations;

namespace CinemaProject.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Img { get; set; } = "defaultcinema.png";
        public string? Description { get; set; }

        public bool Status { get; set; } = true;
        public ICollection<Movie> Movies { get; set; }

        public ICollection<ActorCinema> ActorCinema { get; set; } = new List<ActorCinema>();
        public ICollection<MovieCinema> MovieCinemas { get; set; } = new List<MovieCinema>();

    }
}
