using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaProject.Data.EntityConfigurations
{
    public class ActorMovieEntityTypeConfiguration : IEntityTypeConfiguration<ActorMovie>
    {
        public void Configure(EntityTypeBuilder<ActorMovie> builder)
        {
            builder.HasKey(e => new { e.MovieId, e.ActorId });
        }
    }
}
