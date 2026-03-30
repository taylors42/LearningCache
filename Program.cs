using LearningCache.Data;
using LearningCache.Scripts;

namespace LearningCache;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddMemoryCache();
        
        builder.Services.AddDbContext<DataContext>();
        
        var app = builder.Build();

        // Seed data if requested
        if (args.Contains("--seed") || args.Contains("-s"))
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var count = args.FirstOrDefault(a => int.TryParse(a, out _)) != null 
                ? int.Parse(args.First(a => int.TryParse(a, out _))) 
                : 50;
            SeedData.SeedRandomClients(context, count);
        }

        app.MapControllers();

        app.Run();
    }
}
