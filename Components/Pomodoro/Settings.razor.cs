using Microsoft.AspNetCore.Components;
using Pomodroido.Models.Pomodoro.Settings;

namespace Pomodroido.Components.Pomodoro;

public class SettingsComponentBase : ComponentBase
{
    #region Params
    [Parameter] public EventCallback<SettingsModel> OnSuccessfullSubmit { get; set; }
    #endregion
    [SupplyParameterFromForm] protected SettingsModel SettingsFormModel { get; set; } = new();


    protected async Task SubmitForm()
    {
        await OnSuccessfullSubmit.InvokeAsync(SettingsFormModel);
    }
}
