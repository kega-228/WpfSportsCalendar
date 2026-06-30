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

namespace WpfSportsCalendar.Windows
{
    public partial class MainWindow : Window
    {
        private readonly string _currentUsername;
        public MainWindow(string username)
        {
            InitializeComponent();
            
            WelcomeText.Text = $"Ласкаво просимо, {username}!";
            _currentUsername = username;
        }

        private void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            ClubsWindow clubsWin = new ClubsWindow(_currentUsername);
            clubsWin.Owner = this;
            clubsWin.ShowDialog();
        }
    }
}