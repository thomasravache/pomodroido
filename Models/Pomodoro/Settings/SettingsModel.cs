using System.ComponentModel.DataAnnotations;

namespace Pomodroido.Models.Pomodoro.Settings;

public record SettingsModel
{
    [Required]
    public double PomodoroDurationInMinutes { get; set; } = 25;
    [Required]
    public double RestDurationInMinutes { get; set; } = 5;
    [Required]
    public double LongRestDurationInMinutes { get; set; } = 15;
    [Required]
    public int PomodorosCountToLongRest { get; set; } = 4;
}
