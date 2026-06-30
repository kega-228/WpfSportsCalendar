using System.IO;
using System.Windows;
using WpfSportsCalendar.Database;
using System.Linq;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace WpfSportsCalendar.Windows;

public partial class AuthorizationWindow : Window
{
    public AuthorizationWindow()
    {
        InitializeComponent();
    }

    private void CreateAccount(object sender, RoutedEventArgs e)
    {
        LoginScreen.Visibility = Visibility.Collapsed;
        SignupScreen.Visibility = Visibility.Visible;   
        this.Height = 400;
        this.Width = 300;
    }

    private async void LoginButton(object sender, RoutedEventArgs e)
    {
        string login = LoginInput.Text.Trim();
        string password = PasswordInput.Password;
        using (AppDbContext context = new AppDbContext())
        {
            var user = await context.Users.FirstOrDefaultAsync(l => l.Login == login);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                MessageBox.Show("Неправильний логін або пароль!");
                return;
            }
            if (RememberMeCheck.IsChecked == true) 
            { 
                CreateSession(user);
            }

            OpenMainWindow(user.Login);
        }
    }

    private void RegistrButton(object sender, RoutedEventArgs e)
    {
        string login = SignupLoginInput.Text.Trim();
        string password = SignupPasswordInput.Password;
        string RepeatPassword = SignupRepeatPasswordInput.Password;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(RepeatPassword)){
            
            MessageBox.Show("Всі поля обов'язкові");
        return;
        }

        if (login.Length < 3 || login.Length > 20)
        {
            MessageBox.Show("Логін має бути від 3 до 20 символів");
            return;
        }
        if (password.Length < 6)
        {
            MessageBox.Show("Пароль має бути від 6 символів");
            return;
        }

        if (password != RepeatPassword)
        {
            MessageBox.Show("Паролі не співпадають");
            return;
        }

        using (AppDbContext context = new AppDbContext())
        {
            if (context.Users.Any(l => l.Login == login))
            {
                MessageBox.Show("Користувач з таким ім'ям уже існує");
                return;
            }

            User user = new User
            {
                Login = login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            context.Users.Add(user);
            context.SaveChanges();
            if (RememberMeCheck.IsChecked == true)
            {
                CreateSession(user);
            }

            MessageBox.Show("Акаунт успішно створено!");
            OpenMainWindow(user.Login);
        }
    }

    private void CreateSession(User user)
    {
        Guid guid = Guid.NewGuid();
        File.WriteAllText("./session.txt", guid.ToString());
        UserSession cookie = new UserSession
        {
            UserId = user.Id,
            DeviceGuid = guid,
            CreatedAt = DateTime.Now,
            ExpiresAt = DateTime.Now.AddDays(30)
        };
        using (AppDbContext context = new AppDbContext())
        {
            context.UserSessions.Add(cookie);
            context.SaveChanges();
        }
    }
    
    private void OpenMainWindow(string username)
    {
        MainWindow mainWin = new MainWindow(username);
        mainWin.Show();
        this.Close();
    }
}