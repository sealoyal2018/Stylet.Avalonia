using Stylet.Avalonia;
using System;
using System.Collections.Generic;

namespace Stylet.Samples.NavigationController.Pages;

public class ShellViewModel : Conductor<IScreen>, INavigationControllerDelegate
{
    public HeaderViewModel HeaderViewModel { get; }

    public ShellViewModel(HeaderViewModel headerViewModel)
    {
        this.HeaderViewModel = headerViewModel ?? throw new ArgumentNullException(nameof(headerViewModel));
    }

    public void NavigateTo(IScreen screen)
    {
        this.ActivateItem(screen);
    }
}