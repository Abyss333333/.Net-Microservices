using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static IServiceCollection AddPrepPopulation(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
        public static IApplicationBuilder PrepPopulation(this IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>()!, isProd);
            }
            return app;
        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("--> Attempt to apply Migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not write migrations due to {ex.Message}");
                }
            }
            if (!context.Platforms.Any())
            {
                Console.WriteLine("Seeding Data...");
                context.Platforms.AddRange(
                    new Platform{Name="Dot Net", Publisher ="Microsoft", Cost = "Free"},
                    new Platform{Name="SQL Server Express", Publisher ="Microsoft", Cost = "Free"},
                    new Platform{Name="Kubernetes", Publisher ="Cloud Native Computing Foundation", Cost = "Free"}
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We Already Have Data");
            }
        }
    }
}