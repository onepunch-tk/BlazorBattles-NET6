using System.Net.Http.Json;

namespace BlazorBattles.Client.Services;

public interface IBananaService
{
    event Action OnChange;
    int Bananas { get; set; }
    void EatBananas(int amount);
    Task AddBananas(int amount);
    Task GetBananas();
}

public class BananaService : IBananaService
{
    private readonly HttpClient _http;
    public event Action? OnChange;
    public int Bananas { get; set; } = 0;

    public BananaService(HttpClient http)
    {
        _http = http;
    }
    
    public void EatBananas(int amount)
    {
        Bananas -= amount;
        BananasChanged();
    }

    public async Task AddBananas(int amount)
    {
        var result = await _http.PutAsJsonAsync<int>("api/user/addbananas", amount);
        Bananas = await result.Content.ReadFromJsonAsync<int>();
        BananasChanged();
    }

    public async Task GetBananas()
    {
        Bananas = await _http.GetFromJsonAsync<int>("/api/user/getbananas");
        BananasChanged();
    }

    private void BananasChanged() => OnChange?.Invoke();
}