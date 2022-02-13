using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using BlazorBattles.Client.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorBattles.Client;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly HttpClient _http;
    private readonly IBananaService _bananaService;

    public CustomAuthStateProvider(ILocalStorageService localStorageService, HttpClient http,
        IBananaService bananaService)
    {
        _localStorageService = localStorageService;
        _http = http;
        _bananaService = bananaService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // var state = new AuthenticationState(new ClaimsPrincipal());
        string authToken = await _localStorageService.GetItemAsStringAsync("authToken");
        var identity = new ClaimsIdentity();
        _http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrEmpty(authToken))
        {
            // identity = new ClaimsIdentity(new[]
            // {
            //     new Claim(ClaimTypes.Name, "tkHa")
            // }, "test authentication type");
            try
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                    authToken.Replace("\"",""));
                await _bananaService.GetBananas();
            }
            catch (Exception e)
            {
                await _localStorageService.RemoveItemAsync("authToken");
                identity = new ClaimsIdentity();
            }
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));
        //return  new AuthenticationState(user);
        return state;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, Object>>(jsonBytes);
        var claims = keyValuePairs.Select(kvp =>
            new Claim(kvp.Key, kvp.Value.ToString()));

        return claims;
    }
}