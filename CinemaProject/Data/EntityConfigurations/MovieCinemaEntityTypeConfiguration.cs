using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaProject.Data.EntityConfigurations
{
    public class MovieCinemaEntityTypeConfiguration : IEntityTypeConfiguration<MovieCinema>
    {
        public void Configure(EntityTypeBuilder<MovieCinema> builder)
        {
            builder.HasKey(e => new { e.MovieId, e.CinemaId });
        }
    }
}
