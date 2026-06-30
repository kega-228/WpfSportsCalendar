using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WpfSportsCalendar.Services;

public class ApiClub
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
}
public class TeamContainer
{
    public ApiClub Team { get; set; } = null!;
}

public class ApiClubResponse
{
    public List<TeamContainer> Response { get; set; } = new();
}

public class FootballApiService
{
    private readonly HttpClient _httpClient;
    private static readonly string _apiKey = AppConfig.ApiKey;

    public FootballApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("x-apisports-key", _apiKey);

        
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
}