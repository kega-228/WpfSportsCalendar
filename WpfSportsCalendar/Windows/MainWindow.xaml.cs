using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfSportsCalendar.Database;
using WpfSportsCalendar.Services;

namespace WpfSportsCalendar.Windows
{
    public partial class MainWindow : Window
    {
        private readonly string _currentUsername;
        private readonly FootballApiService _footballApiService;
        public MainWindow(string username)
        {
            InitializeComponent();
            _currentUsername = username;
            _footballApiService = new FootballApiService();
            
            WelcomeText.Text = $"Ласкаво просимо, {username}!";
            LoadDashboardMatches();
        }

        private async void LoadDashboardMatches()
        {
            List<int> subscribedClubIds;

            using (AppDbContext context = new AppDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Login == _currentUsername);
                if (user == null) return;

                subscribedClubIds = context.Subscriptions
                    .Where(s => s.UserId == user.Id)
                    .Select(s => s.ClubId)
                    .ToList();
            }

            if (subscribedClubIds.Count == 0) return;

            var allMatches = await _footballApiService.GetMatchesAsync(subscribedClubIds);
            
            if (allMatches.Count == 0) return;
            var sortedMatches = allMatches.OrderBy(m => m.RawDate).ToList();

            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc); // Додано із-за технічних обмежень API
            var dashboardMatches = new List<DisplayMatch>();

            var lastFinishedMatch = sortedMatches
                .LastOrDefault(m => m.IsFinished && m.RawDate <= now);

            if (lastFinishedMatch != null)
            {
                dashboardMatches.Add(lastFinishedMatch);
            }

            var upcomingMatches = sortedMatches
                .Where(m => /*!m.IsFinished &&*/ m.RawDate > now)
                .Take(2)
                .ToList();

            // Додано із-за технічних обмежень API
            foreach (var match in upcomingMatches)
            {
                match.StatusText = "Не почався";
                match.IsFinished = false;
                match.ScoreOrTime = match.RawDate.ToString("HH:mm");
                dashboardMatches.Add(match);
            }

            MatchesItemsControl.ItemsSource = dashboardMatches;
        }

        private void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            ClubsWindow clubsWin = new ClubsWindow(_currentUsername);
            clubsWin.Owner = this;
            clubsWin.ShowDialog();
            LoadDashboardMatches();
        }
        
        private void UnsubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            UnsubscribeWindow unsubWin = new UnsubscribeWindow(_currentUsername);
            unsubWin.Owner = this;
            unsubWin.ShowDialog();

            LoadDashboardMatches();
        }
    }
}