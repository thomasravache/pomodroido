using Microsoft.JSInterop;

namespace Pomodroido.Services.JavaScript;

public class JsNotificationService(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private IJSObjectReference? _module;
    private bool _disposed;


    public async Task TriggerNotification(string title, string message)
    {
        var module = await GetModule();

        await module.InvokeVoidAsync("showNotification", title, message);
    }

    public async Task RequestNotificationPermission()
    {
        var module = await GetModule();

        await module.InvokeVoidAsync("requestNotificationPermission");
    }

    public async Task PlayNotificationSound()
    {
        var module = await GetModule();

        await module.InvokeVoidAsync("playNotificationSound");
    }

    private async Task<IJSObjectReference> GetModule()
    {
        _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/notifications.js");

        return _module;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            if (_module is not null)
                await _module.DisposeAsync();
        }
        finally
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
