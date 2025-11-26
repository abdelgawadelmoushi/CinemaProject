
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaProject.Data.EntityConfiguration
{
    public class MovieSubImagesEntityTypeConfiguration : IEntityTypeConfiguration<MovieSubImages>
    {
        public void Configure(EntityTypeBuilder<MovieSubImages> builder)
        {
            builder.HasKey(e=>new {e.MovieId, e.Img});
        }
    }
}
