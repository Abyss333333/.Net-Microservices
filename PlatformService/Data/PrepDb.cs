using System.Text.Json.Serialization;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static IServiceCollection AddPrepPopulation(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
        public static IApplicationBuilder PrepPopulation(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>()!);
            }
            return app;
        }

        private static void SeedData(AppDbContext context)
        {
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