using System.ComponentModel.DataAnnotations;

namespace Pomodroido.Models.Pomodoro.Settings;

public record SettingsModel
{
    [Required]
    public int PomodoroDurationInMinutes { get; set; } = 25;
    [Required]
    public int RestDurationInMinutes { get; set; } = 5;
    [Required]
    public int LongRestDurationInMinutes { get; set; } = 15;
    [Required]
    public int PomodorosCountToLongRest { get; set; } = 4;
}
