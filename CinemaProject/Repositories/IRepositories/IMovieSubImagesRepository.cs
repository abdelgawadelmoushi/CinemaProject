using NuGet.Protocol.Core.Types;
using System.Linq.Expressions;

namespace CinemaProject.Repositories.IRepositories
{
    public interface IMovieSubImagesRepository : IRepository<MovieSubImages>
    {

        Task AddRangeAsync(IEnumerable<MovieSubImages> items, CancellationToken cancellationToken = default);
        void RemoveRange(IEnumerable<MovieSubImages> items);
    }
}
