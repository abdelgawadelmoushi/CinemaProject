using System.ComponentModel.DataAnnotations;

namespace CinemaProject.Models
{
    public class MovieSubImages
    {
        public int Id { get; set; }

        [Required]
        public string Img { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
