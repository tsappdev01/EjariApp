using DIP.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components;

namespace DIP.Blazor;

public abstract class DIPComponentBase : AbpComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    protected DIPComponentBase()
    {
        LocalizationResource = typeof(DIPResource);
    }

    public async Task OnInvalid()
    {
        await InvokeAsync(StateHasChanged);
        await Task.Delay(1);
        await JSRuntime.InvokeVoidAsync("eval", @"
        const el = document.querySelector('.validation-message');
        if (el) {
            el.scrollIntoView({ behavior: 'smooth', block: 'center' });
            el.focus();
        }
    ");

    }
}
