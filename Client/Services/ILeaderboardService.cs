using System.Net.Http.Json;
using BlazorBattles.Shared;

namespace BlazorBattles.Client.Services;

public interface ILeaderboardService
{
    IList<UserStatistic> Leaderboard { get; set; }
    Task GetLeaderboard();
}

public class LeaderboardService : ILeaderboardService
{
    private readonly HttpClient _http;

    public LeaderboardService(HttpClient http)
    {
        _http = http;
    }
    
    public IList<UserStatistic> Leaderboard { get; set; }
    
    public async Task GetLeaderboard()
    {
        Leaderboard = await _http.GetFromJsonAsync<IList<UserStatistic>>("/api/user/leaderboard");
    }
} 