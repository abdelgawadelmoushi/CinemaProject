using NuGet.Protocol.Core.Types;
using System.Linq.Expressions;

namespace CinemaProject.Repositories.IRepositories
{
    public interface IActorMovieRepository : IRepository<ActorMovie>
    {

        Task AddRangeAsync(IEnumerable<ActorMovie> items, CancellationToken cancellationToken = default);
        void RemoveRange(IEnumerable<ActorMovie> items);
    }
}
