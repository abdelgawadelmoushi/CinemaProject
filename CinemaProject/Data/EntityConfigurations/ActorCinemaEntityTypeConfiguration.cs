using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaProject.Data.EntityConfigurations
{
    public class ActorCinemaEntityTypeConfiguration : IEntityTypeConfiguration<ActorCinema>
    {
        public void Configure(EntityTypeBuilder<ActorCinema> builder)
        {
            builder.HasOne(ac => ac.Actor)
                   .WithMany(a => a.ActorCinema)
                   .HasForeignKey(ac => ac.ActorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ac => ac.Cinema)
                   .WithMany(c => c.ActorCinema)
                   .HasForeignKey(ac => ac.CinemaId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
