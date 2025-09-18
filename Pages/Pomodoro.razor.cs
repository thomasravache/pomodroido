using Microsoft.AspNetCore.Components;
using Pomodroido.Enums;
using Pomodroido.Models.Pomodoro.Settings;
using Pomodroido.Models.Pomodoro.Timer;

namespace Pomodroido.Pages;

public class PomodoroComponentBase : ComponentBase
{
    protected Components.Pomodoro.Timer? TimerRef { get; set; }
    protected PomodoroCycleModel Cycle { get; set; } = new();
    protected PomodoroIntervalModel? CurrentInterval { get; set; }
    protected SettingsModel SettingsModel { get; set; } = new();


    protected override void OnInitialized()
    {
        FillPomodoroCycle(SettingsModel);
    }

    protected void FillNextCurrentInterval()
    {
        var cycleInterval = Cycle.Intervals.FirstOrDefault(x => x.Order == CurrentInterval?.Order);

        if (cycleInterval is null) return;

        cycleInterval.Done = true;

        CurrentInterval = Cycle.Intervals.FirstOrDefault(x => !x.Done);

        if (Cycle.Intervals.All(x => x.Done))
        {
            FillPomodoroCycle(SettingsModel);
            CurrentInterval = Cycle.Intervals.FirstOrDefault(x => !x.Done);
        }

        StateHasChanged();
    }

    protected void FillPomodoroCycle(SettingsModel settings)
    {
        TimerRef?.ResetTimer();
        
        Cycle.Intervals.Clear();
        SettingsModel = settings with { };
        int lastOrder = 1;

        for (var index = 1; index <= settings.PomodorosCountToLongRest; index++)
        {
            bool isLastOne = index == settings.PomodorosCountToLongRest;

            Cycle.Intervals.Add(new()
            {
                TimeInMinutes = settings.PomodoroDurationInMinutes,
                Type = ECycleType.Pomodoro,
                Order = lastOrder
            });

            lastOrder++;

            if (isLastOne)
                Cycle.Intervals.Add(new()
                {
                    TimeInMinutes = settings.LongRestDurationInMinutes,
                    Type = ECycleType.LongRest,
                    Order = lastOrder
                });
            else
                Cycle.Intervals.Add(new()
                {
                    TimeInMinutes = settings.RestDurationInMinutes,
                    Type = ECycleType.Rest,
                    Order = lastOrder
                });

            lastOrder++;
        }

        Cycle.Intervals = Cycle.Intervals.OrderBy(x => x.Order).ToList();
        CurrentInterval = Cycle.Intervals.FirstOrDefault();
    }
}
