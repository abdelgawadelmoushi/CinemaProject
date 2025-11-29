using CinemaProject.Models;
using CinemaProject.Repositories;
using CinemaProject.Repositories.IRepositories;

public class MovieSubImagesRepository : Repository<MovieSubImages>, IMovieSubImagesRepository
{
    protected readonly ApplicationDbContext _context;

    public MovieSubImagesRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void RemoveRange(IEnumerable<MovieSubImages> items)
    {
        _context.MovieSubImages.RemoveRange(items);
    }

    public async Task AddRangeAsync(IEnumerable<MovieSubImages> items, CancellationToken cancellationToken = default)
    {
        await _context.MovieSubImages.AddRangeAsync(items, cancellationToken);
    }
}
