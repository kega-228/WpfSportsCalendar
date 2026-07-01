namespace WpfSportsCalendar.Services;

public class DisplayMatch
{
    public string DateText { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public bool IsFinished { get; set; }
        
    public string HomeTeamName { get; set; } = string.Empty;
    public string HomeTeamLogo { get; set; } = string.Empty;
    public string AwayTeamName { get; set; } = string.Empty;
    public string AwayTeamLogo { get; set; } = string.Empty;

    public string ScoreOrTime { get; set; } = string.Empty;
    public DateTime RawDate { get; set; }
}