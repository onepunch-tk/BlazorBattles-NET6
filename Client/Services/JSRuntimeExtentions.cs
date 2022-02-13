using Microsoft.JSInterop;

namespace BlazorBattles.Client.Services;

public static class JSRuntimeExtentions
{
    public static async Task ConsolLog(this IJSRuntime jsRuntime, string message)
    {
        await jsRuntime.InvokeVoidAsync("ConsolLog2",message);
    }
}