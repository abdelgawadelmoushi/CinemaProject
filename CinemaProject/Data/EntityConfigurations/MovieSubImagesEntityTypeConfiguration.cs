using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaProject.Data.EntityConfigurations
{
    public class MovieSubImagesEntityTypeConfiguration : IEntityTypeConfiguration<MovieSubImages>
    {
        public void Configure(EntityTypeBuilder<MovieSubImages> builder)
        {
            builder
                .HasOne(msi => msi.Movie)
                .WithMany(m => m.MovieSubImages)
                .HasForeignKey(msi => msi.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
