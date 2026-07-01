namespace WpfSportsCalendar.Services;

public class FixtureItem
{
    public FixtureDetails Fixture { get; set; } = null!;
    public TeamsDetails Teams { get; set; } = null!;
    public GoalsDetails Goals { get; set; } = null!;
}