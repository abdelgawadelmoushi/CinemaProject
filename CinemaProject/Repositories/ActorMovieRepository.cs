using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CinemaProject.Repositories.IRepositories
{
    public class ActorMovieRepository : Repository<ActorMovie> , IActorMovieRepository
    {
        protected readonly ApplicationDbContext _context;

        public ActorMovieRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

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
