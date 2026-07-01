namespace WpfSportsCalendar.Services;

public class FixtureDetails
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public StatusDetails Status { get; set; } = null!;
}