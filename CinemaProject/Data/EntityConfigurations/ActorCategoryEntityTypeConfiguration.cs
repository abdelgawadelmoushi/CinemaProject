using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace CinemaProject.Data.EntityConfigurations
{
    public class ActorCategoryEntityTypeConfiguration : IEntityTypeConfiguration<ActorCategory>
    {
        public void Configure(EntityTypeBuilder<ActorCategory> builder)
        {
            builder.HasOne(ac => ac.Actor)
                      .WithMany(a => a.ActorCategories)
                      .HasForeignKey(ac => ac.ActorId)
                      .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ac => ac.Category)
                   .WithMany(c => c.ActorCategories)
                   .HasForeignKey(ac => ac.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
