namespace Pomodroido.Models.Pomodoro.Timer;

public record PomodoroCycleModel
{
    public List<PomodoroIntervalModel> Intervals { get; set; } = [];
    public bool Done { get; set; } = false;
}
