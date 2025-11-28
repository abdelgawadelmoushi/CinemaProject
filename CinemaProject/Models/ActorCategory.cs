namespace CinemaProject.Models
{
    public class ActorCategory
    {
        public int Id { get; set; }
        public int ActorId { get; set; }
        public Actor Actor { get; set; } =  new();
        public int CategoryId { get; set; }
        public Category Category { get; set; } = new();
    }
}
