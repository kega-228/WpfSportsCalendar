using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using WpfSportsCalendar.Database;
using WpfSportsCalendar.Windows;

namespace WpfSportsCalendar;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        const string sessionFile = "./session.txt";
        
        if (File.Exists(sessionFile))
        {
            string raw = File.ReadAllText(sessionFile).Trim();
            
            if (Guid.TryParse(raw, out Guid deviceGuid))
            {
                using AppDbContext context = new AppDbContext();
 
                var session = await context.UserSessions
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s =>
                        s.DeviceGuid == deviceGuid &&
                        s.ExpiresAt > DateTime.Now);

                if (session != null)
                {
                    session.ExpiresAt = DateTime.Now.AddDays(30);
                    await context.SaveChangesAsync();
                    
                    MainWindow mainWin = new MainWindow(session.User.Login);
                    mainWin.Show();
                }

                return;
            }
            
            File.Delete(sessionFile);
        }
        AuthorizationWindow authWin = new AuthorizationWindow();
        authWin.Show();
    }
    public App()
    {
        
    }
}