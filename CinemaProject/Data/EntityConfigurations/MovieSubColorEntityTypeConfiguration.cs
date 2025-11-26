using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaProject.Data.EntityConfigurations
{
    public class movieColorEntityTypeConfiguration : IEntityTypeConfiguration<movieColor>
    {
        public void Configure(EntityTypeBuilder<movieColor> builder)
        {
            builder.HasKey(e => new { e.MovieId, e.Color });
        }
    }
}
