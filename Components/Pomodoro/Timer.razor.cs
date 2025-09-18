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

    private TimeSpan? _remainingTimeWhenStop;
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

        if (_remainingTimeWhenStop.HasValue)
        {
            _startTime = DateTime.Now - (IntervalDuration - _remainingTimeWhenStop.Value);
            _remainingTimeWhenStop = null;
        }
        else
        {
            _startTime = DateTime.Now;
        }

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
            ResetTimer();

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

        _remainingTimeWhenStop = RemainingTime;

        _uiTimer?.Stop();
        _uiTimer?.Dispose();
        _uiTimer = null;
    }

    protected async Task OnClickResetTimer()
    {
        ResetTimer();
        StateHasChanged();
        await OnClickResetTimerFinished.InvokeAsync();
    }

    public void ResetTimer()
    {
        StopTimer();
        _startTime = null;
        _remainingTimeWhenStop = null;
        StateHasChanged();
    }

    
    public async ValueTask DisposeAsync()
    {
        ResetTimer();
        GC.SuppressFinalize(this);
        await Task.CompletedTask;
    }
}
