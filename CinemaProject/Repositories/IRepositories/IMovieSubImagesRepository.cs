using CinemaProject.Models;

namespace CinemaProject.Repositories.IRepositories
{
    public interface IMovieSubImagesRepository : IRepository<MovieSubImages>
    {
        void RemoveRange(IEnumerable<MovieSubImages> items);
        Task AddRangeAsync(IEnumerable<MovieSubImages> items, CancellationToken cancellationToken = default);
    }
}
