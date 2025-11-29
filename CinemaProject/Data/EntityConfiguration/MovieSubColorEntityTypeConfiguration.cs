using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CinemaProject.Data.EntityConfiguration
{
    public class ActorMovieEntityTypeConfiguration : IEntityTypeConfiguration<movieColor>
    {
        public ActorMovieEntityTypeConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<movieColor> builder)
        {
            builder.HasKey(e => new { e.MovieId, e.Color });
        }
    }
}
