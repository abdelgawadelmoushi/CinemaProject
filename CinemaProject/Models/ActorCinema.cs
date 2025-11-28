namespace CinemaProject.Models
{
    public class ActorCinema
    {
        public int Id { get; set; }
        public int ActorId { get; set; }
        public Actor Actor { get; set; } =  new();
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; } = new();
    }
}
