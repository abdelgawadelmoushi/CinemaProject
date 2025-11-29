using CinemaProject.Repositories;
using CinemaProject.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CinemaProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container. 
            builder.Services.AddControllersWithViews();

            //builder.Services.AddSession(opt => {
            //    opt.IdleTimeout = TimeSpan.FromSeconds(5);
            //});
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer("Data Source=.;Initial Catalog=CinemaProject;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"));

            //»‰ÿ·» „‰ ”Ì ‘«—»  ÷Ì›·‰« ‰ÿ«ﬁ ÃœÌœ «Ê· „«   ‘Ê› «·‰Ê⁄ «··Ì Ã«Ì·Â«  ÷Ì› «Ê»ÃÌﬂ  „‰ ‰›” «·‰Ê⁄
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IMovieSubImagesRepository, MovieSubImagesRepository>();
            builder.Services.AddScoped<IActorMovieRepository, ActorMovieRepository>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            //app.UseSession();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
