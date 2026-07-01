using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WpfSportsCalendar.Services;

public class FootballApiService
{
    private readonly HttpClient _httpClient;
    private static readonly string _apiKey = AppConfig.ApiKey;

    public FootballApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("x-apisports-key", _apiKey);

        
    }

    public async Task<List<DisplayMatch>> GetMatchesAsync(List<int> clubsIds)
    {
        var allMatches = new List<DisplayMatch>();
        if (clubsIds == null || clubsIds.Count == 0) return allMatches;
        try
        {
            foreach (var id in clubsIds)
            {
                string url = $"https://v3.football.api-sports.io/fixtures?team={id}&season=2024";//season=2024 із-за технічних обмежень API";"
                var result = await _httpClient.GetFromJsonAsync<FixtureResponse>(url);

                if (result?.Response != null)
                {
                    foreach (var item in result.Response)
                    {
                        bool isFt = item.Fixture.Status.Short == "FT";
                            
                        var match = new DisplayMatch
                        {
                            RawDate = item.Fixture.Date.ToLocalTime(),
                            DateText = item.Fixture.Date.ToLocalTime().ToString("dd MMMM yyyy"),
                            IsFinished = isFt,
                            StatusText = isFt ? "Завершен" : "Не начался",
                            HomeTeamName = item.Teams.Home.Name == "Polessya" ? "Полісся" : item.Teams.Home.Name,
                            HomeTeamLogo = item.Teams.Home.Logo,
                            AwayTeamName = item.Teams.Away.Name == "Polessya" ? "Полісся" : item.Teams.Away.Name,
                            AwayTeamLogo = item.Teams.Away.Logo,
                            ScoreOrTime = isFt 
                                ? $"{item.Goals.Home}:{item.Goals.Away}" 
                                : item.Fixture.Date.ToLocalTime().ToString("HH:mm")
                        };
                            
                        allMatches.Add(match);
                    }
                }
            }
        }
        catch
        {
        }
        return allMatches;
    }
    
    public async Task<List<ApiClub>> SearchClubsAsync(string query)
    {
        try
        {
            string url = $"https://v3.football.api-sports.io/teams?search={query}";

            var result = await _httpClient.GetFromJsonAsync<ApiClubResponse>(url);

            var clubsList = new List<ApiClub>();

            if (result?.Response != null)
            {
                foreach (var container in result.Response)
                {
                    var club = container.Team;

                    if (club.Name == "Polessya")
                    {
                        club.Name = "Полісся Житомир";
                    }

                    clubsList.Add(club);
                }
                clubsList = clubsList
                    .OrderByDescending(c => c.Name.Equals(query, StringComparison.OrdinalIgnoreCase) && c.Name.Equals(c.Country, StringComparison.OrdinalIgnoreCase))
                    .ThenByDescending(c => c.Name.Equals(c.Country, StringComparison.OrdinalIgnoreCase))
                    .ThenBy(c => c.Name)
                    .ToList();
            }

            return clubsList;
        }
        catch
        {
            return new List<ApiClub>();
        }
    }
    
    public async Task<ApiClub?> GetClubByIdAsync(int clubId)
    {
        try
        {
            string url = $"https://v3.football.api-sports.io/teams?id={clubId}";

            var result = await _httpClient.GetFromJsonAsync<ApiClubResponse>(url);
        
            return result?.Response?.FirstOrDefault()?.Team;
        }
        catch
        {
            return null;
        }
    }
}