using System.Net;
using System.Net.Http.Json;
using BlazorBattles.Shared;
using Blazored.Toast.Services;

namespace BlazorBattles.Client.Services;

public interface IUnitService
{
    IList<Unit>? Units { get; set; }
    IList<UserUnit> MyUnits { get; set; }
    Task AddUnit(int unitId);
    Task LoadUnitsAsync();
    Task LoadUserUnitAsync();
    Task ReviveArmy();
}

public class UnitService : IUnitService
{
    private readonly IToastService _toastService;
    private readonly HttpClient _httpClient;
    private readonly IBananaService _bananaService;

    public UnitService(IToastService toastService, HttpClient httpClient, IBananaService bananaService)
    {
        _toastService = toastService;
        _httpClient = httpClient;
        _bananaService = bananaService;
    }

    public IList<Unit>? Units { get; set; } = new List<Unit>();

    public IList<UserUnit> MyUnits { get; set; } = new List<UserUnit>();

    public async Task AddUnit(int unitId)
    {
        var unit = Units?.First(unit => unit.Id.Equals(unitId));
        var result = await _httpClient.PostAsJsonAsync<int>("api/userunit", unitId);
        if (result.StatusCode != HttpStatusCode.OK)
        {
            _toastService.ShowError(await result.Content.ReadAsStringAsync());
        }
        else
        {
            await _bananaService.GetBananas();
            _toastService.ShowSuccess($"Your {unit.Title} han been built!", "Unit built!");
        }
        
    }

    public async Task LoadUnitsAsync()
    {
        if (Units is not {Count: not 0})
        {
            Units = await _httpClient.GetFromJsonAsync<IList<Unit>>("api/Unit");
        }
    }

    public async Task LoadUserUnitAsync()
    {
        MyUnits = await _httpClient.GetFromJsonAsync<IList<UserUnit>>("api/userunit");
    }

    public async Task ReviveArmy()
    {
        var result = await _httpClient.PostAsJsonAsync<string>("api/userunit/revive", null);
        
        if(result.StatusCode == HttpStatusCode.OK)
            _toastService.ShowSuccess(await result.Content.ReadAsStringAsync());
        else
            _toastService.ShowError(await result.Content.ReadAsStringAsync());

        await LoadUserUnitAsync();
        await _bananaService.GetBananas();
    }
}