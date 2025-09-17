using Pomodroido.Enums;

namespace Pomodroido.Models.Pomodoro.Timer;

public record PomodoroIntervalModel
{
    public required ECycleType Type { get; init; } = ECycleType.Pomodoro;
    public bool Done { get; set; } = false;
    public required int TimeInMinutes { get; init; } = 25;
    public required int Order { get; set; } = 1;
}
