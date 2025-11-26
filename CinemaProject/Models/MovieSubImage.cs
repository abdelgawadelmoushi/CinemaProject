using Microsoft.EntityFrameworkCore;

namespace CinemaProject.Models
{
    public class MovieSubImages
    {
        public int Id { get; set; }        
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;
        public string Img { get; set; } = string.Empty;
    }
}
