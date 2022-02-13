using System.Net.Http.Json;
using BlazorBattles.Shared;

namespace BlazorBattles.Client.Services;

public interface IBattleService
{
    BattleResult LastBattle { get; set; }
    IList<BattleHistoryEntry> Histories { get; set; }
    Task<BattleResult> StartBattle(int oppnentId);
    Task GetHistory();
}

public class BattleService : IBattleService
{
    private readonly HttpClient _http;

    public BattleService(HttpClient http)
    {
        _http = http;
    }

    public BattleResult LastBattle { get; set; } = new BattleResult();
    public IList<BattleHistoryEntry> Histories { get; set; } = new List<BattleHistoryEntry>();

    public async Task<BattleResult> StartBattle(int oppnentId)
    {
        var respone = await _http.PostAsJsonAsync("api/battle", oppnentId);
        LastBattle = await respone.Content.ReadFromJsonAsync<BattleResult>();
        return LastBattle;
    }

    public async Task GetHistory()
    {
        Histories = await _http.GetFromJsonAsync<BattleHistoryEntry[]>("api/user/history");
    }
}