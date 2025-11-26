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
    }
}
