using System.Windows;
using WpfSportsCalendar.Database;
using WpfSportsCalendar.Services;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Windows.Controls;

namespace WpfSportsCalendar.Windows;

public partial class UnsubscribeWindow : Window
{
    private readonly string _username;
    private readonly FootballApiService _footballApiService;
    
    public UnsubscribeWindow(string username)
    {
        InitializeComponent();
        _username = username;
        _footballApiService = new FootballApiService();
        LoadSubscribedClubs();
    }

    private async void LoadSubscribedClubs()
    {
        List<int> clubsIds;

        using (AppDbContext context = new AppDbContext())
        {
            var user = context.Users.FirstOrDefault(u => u.Login == _username);
            if (user == null) return;
            
            clubsIds = context.Subscriptions
                .Where(s => s.UserId == user.Id)
                .Select(s => s.ClubId)
                .ToList();
        }

        if (clubsIds.Count == 0)
        {
            SubscriptionsListBox.ItemsSource = null;
            MessageBox.Show("Ви не підписані на жоден клуб");
            return;
        }
        var displayClubs = new List<ApiClub>();

        foreach (var id in clubsIds)
        {
            try 
            {
                var team = await _footballApiService.GetClubByIdAsync(id);
                if (team != null)
                {
                    if (team.Name == "Polessya") team.Name = "Полісся";
        
                    displayClubs.Add(team);
                }
            }
            catch { }
            
        }
        SubscriptionsListBox.ItemsSource = displayClubs;
    }

    private void UnsubscribeButton_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button) sender;
        int clubId = (int)button.Tag;
        
        using (AppDbContext context = new AppDbContext())
        {
            var user = context.Users.FirstOrDefault(u => u.Login == _username);
            if (user == null) return;
            
            context.Subscriptions.Remove(context.Subscriptions.FirstOrDefault(s => s.UserId == user.Id && s.ClubId == clubId));
            context.SaveChanges();
            MessageBox.Show("Відписано");
        }
        LoadSubscribedClubs();
    }
}