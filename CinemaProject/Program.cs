using CinemaProject.Repositories;
using CinemaProject.Repositories.IRepositories;
using CinemaProject.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;

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


            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")?? throw new InvalidOperationException("ConnectionString" +
                "Default Connection not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));



            //»‰ÿ·» „‰ ”Ì ‘«—»  ÷Ì›·‰« ‰ÿ«ﬁ ÃœÌœ «Ê· „«   ‘Ê› «·‰Ê⁄ «··Ì Ã«Ì·Â«  ÷Ì› «Ê»ÃÌﬂ  „‰ ‰›” «·‰Ê⁄

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric=false;
                //options.SignIn.RequireConfirmedEmail = false;

            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
                

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IMovieSubImagesRepository, MovieSubImagesRepository>();
            builder.Services.AddScoped<IActorMovieRepository, ActorMovieRepository>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();

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
