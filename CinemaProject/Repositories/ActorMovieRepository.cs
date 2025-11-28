using System.Threading.Tasks;

namespace CinemaProject.Repositories
{
    public class ActorMovieRepository : Repository<ActorMovie>
    {
        private readonly ApplicationDbContext _context = new();

        public void RemoveRange(IEnumerable<ActorMovie> items)
        {
            _context.ActorMovies.RemoveRange(items);
        }

        public async Task AddRangeAsync(IEnumerable<ActorMovie> items, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(items, cancellationToken);
        }
    }
}
