using Microsoft.AspNetCore.Components;
using Pomodroido.Models.Pomodoro.Settings;
using Pomodroido.Services.JavaScript;
using System.Threading.Tasks;
using System.Timers;

namespace Pomodroido.Components.Pomodoro;

public class TimerComponentBase : ComponentBase
{
    #region Injections
    [Inject] private JsNotificationService _jsNotificationService { get; init; } = null!;
    #endregion

    #region Params
    [Parameter] public EventCallback OnFinishedInterval { get; set; }
    [Parameter] public EventCallback OnClickResetTimerFinished { get; set; }
    [Parameter] public TimeSpan RemainingTime { get; set; }
    #endregion

    #region Props
    protected bool IsTimerRunning { get; set; } = false;
    protected string FormattedTime => RemainingTime.ToString(@"hh\:mm\:ss");

    private CancellationTokenSource? _cancellationTokenSource;
    #endregion

    #region LifeCycle
    protected override async Task OnInitializedAsync()
    {
        await _jsNotificationService.RequestNotificationPermission();
    }
    #endregion

    protected async Task StartTimer()
    {
        if (IsTimerRunning) return;

        IsTimerRunning = true;
        _cancellationTokenSource = new();

        try
        {
            while (IsTimerRunning && RemainingTime.TotalSeconds > 0)
            {
                TimeSpan oneSecond = TimeSpan.FromSeconds(1);
                await Task.Delay(oneSecond, _cancellationTokenSource.Token);
                RemainingTime = RemainingTime.Subtract(oneSecond);
                StateHasChanged();
            }

            if (RemainingTime.TotalSeconds <= 0)
            {
                await _jsNotificationService.TriggerNotification("Pomodoro Finalizado!", "Fim da contagem.");
                await _jsNotificationService.PlayNotificationSound();
                ResetTimer();

                await OnFinishedInterval.InvokeAsync();
            }
        }
        catch (TaskCanceledException)
        {
            // do nothing
        }
    }

    protected void StopTimer()
    {
        IsTimerRunning = false;
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    protected async Task OnClickResetTimer()
    {
        ResetTimer();
        await OnClickResetTimerFinished.InvokeAsync();
    }

    private void ResetTimer()
    {
        StopTimer();
        StateHasChanged();
    }
}
