using Microsoft.AspNetCore.Components;
using Pomodroido.Enums;
using Pomodroido.Services.JavaScript;

namespace Pomodroido.Components.Pomodoro;

public class TimerComponentBase : ComponentBase, IAsyncDisposable
{
    #region Injections
    [Inject] private JsNotificationService _jsNotificationService { get; init; } = null!;
    #endregion

    #region Params
    [Parameter] public EventCallback OnFinishedInterval { get; set; }
    [Parameter] public EventCallback OnClickResetTimerFinished { get; set; }
    [Parameter] public TimeSpan IntervalDuration { get; set; }
    [Parameter] public ECycleType Type { get; set; }
    #endregion

    #region Props
    protected bool IsTimerRunning { get; set; } = false;
    protected string FormattedTime => RemainingTime > TimeSpan.Zero
     ? RemainingTime.ToString(@"hh\:mm\:ss")
     : "00:00:00";

    private System.Timers.Timer? _uiTimer;

    private DateTime? _startTime;

    protected TimeSpan RemainingTime =>
        _startTime.HasValue
            ? (_startTime.Value + IntervalDuration - DateTime.Now)
            : IntervalDuration;
    #endregion

    #region LifeCycle
    protected override async Task OnInitializedAsync()
    {
        await _jsNotificationService.RequestNotificationPermission();
    }
    #endregion

    protected void StartTimer()
    {
        if (IsTimerRunning) return;

        IsTimerRunning = true;
        _startTime = DateTime.Now;

        _uiTimer = new System.Timers.Timer(1000);
        _uiTimer.Elapsed += (_, _) => InvokeAsync(UpdateUI);
        _uiTimer.AutoReset = true;
        _uiTimer.Start();

    }

    private async Task UpdateUI()
    {
        StateHasChanged();

        if (RemainingTime <= TimeSpan.Zero)
        {
            StopTimer();

            var (title, message) = Type switch
            {
                ECycleType.Pomodoro => ("Finished Pomodoro", "Time to take a rest!"),
                ECycleType.Rest => ("Finished Rest", "Time to get back to work!"),
                ECycleType.LongRest => ("Finished Long Rest", "Time to get back to work in another cycle!"),
                _ => ("Finished Interval", string.Empty)
            };

            await _jsNotificationService.TriggerNotification(title, message);
            await _jsNotificationService.PlayNotificationSound();
            await OnFinishedInterval.InvokeAsync();
        }
    }




    protected void StopTimer()
    {
        IsTimerRunning = false;

        _uiTimer?.Stop();
        _uiTimer?.Dispose();
        _uiTimer = null;

        _startTime = null;
    }

    protected async Task OnClickResetTimer()
    {
        StopTimer();
        StateHasChanged();
        await OnClickResetTimerFinished.InvokeAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        StopTimer();
        GC.SuppressFinalize(this);
        await Task.CompletedTask;
    }
}
