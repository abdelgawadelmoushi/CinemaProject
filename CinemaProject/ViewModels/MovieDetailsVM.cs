namespace CinemaProject.ViewModels
{
    public class MovieDetailsVM
    {
        public Movie Movie { get; set; }
        public IEnumerable<Movie> RelatedMovies { get; set; }
    }

}
