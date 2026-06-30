namespace WpfSportsCalendar.Database;

public class Subscription
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int ClubId { get; set; }
}