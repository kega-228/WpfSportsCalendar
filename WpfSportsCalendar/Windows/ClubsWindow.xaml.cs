using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfSportsCalendar.Database;
using WpfSportsCalendar.Services;

namespace WpfSportsCalendar.Windows;

public partial class ClubsWindow : Window
{
    private readonly string _username;
    private readonly FootballApiService _footballApiService;
    public ClubsWindow(string username)
    {
        _username = username;
        InitializeComponent();
        _footballApiService = new FootballApiService();
    }

    private async void ExecuteSearch()
    {
        string filter = SearchInput.Text.Trim();
        if (string.IsNullOrEmpty(filter)) return;

        List<ApiClub> apiResults = await _footballApiService.SearchClubsAsync(filter);

        ClubsListBox.ItemsSource = apiResults;
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        ExecuteSearch();
    }

    private void SearchInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ExecuteSearch();
        }
    }

    private void SubscribeClubButton_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button) sender;
        int clubId = (int)button.Tag;

        using (AppDbContext context = new AppDbContext())
        {
            var user = context.Users.FirstOrDefault(u => u.Login == _username);
            
            bool isSubscribed = context.Subscriptions.Any(s => s.UserId == user.Id && s.ClubId == clubId);
            if (isSubscribed)
            {
                MessageBox.Show("Ви вже підписані на цей клуб");
                return;
            }
            
            var subscription = new Subscription
            {
                UserId = user.Id,
                ClubId = clubId
            };
            
            context.Subscriptions.Add(subscription);
            context.SaveChanges();
            MessageBox.Show("Підписано");
        }
    }
}