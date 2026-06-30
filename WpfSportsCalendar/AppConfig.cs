using Microsoft.Extensions.Configuration;
using System.IO;

public static class AppConfig
{
    public static IConfiguration Configuration { get; }

    static AppConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<WpfSportsCalendar.App>(); 

        Configuration = builder.Build();
    }

    public static string ConnectionString => 
        Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    public static string ApiKey => Configuration["ApiKey"] ?? string.Empty;
}