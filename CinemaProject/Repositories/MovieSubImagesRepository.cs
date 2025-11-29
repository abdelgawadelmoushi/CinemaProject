using CinemaProject.Repositories.IRepositories;
using System.Threading.Tasks;

namespace CinemaProject.Repositories
{
    public class MovieSubImagesRepository : Repository<MovieSubImages>, IMovieSubImagesRepository
    {

        public MovieSubImagesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void RemoveRange(IEnumerable<MovieSubImages> items)
        {
            _context.MovieSubImages.RemoveRange(items);
        }

        public async Task AddRangeAsync(IEnumerable<MovieSubImages> items, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(items, cancellationToken);
        }
    }
}
