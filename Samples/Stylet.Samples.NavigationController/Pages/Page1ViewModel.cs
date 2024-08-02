using Stylet.Avalonia;
using System;

namespace Stylet.Samples.NavigationController.Pages;

public class Page1ViewModel : Screen
{
    private readonly INavigationController navigationController;

    public Page1ViewModel(INavigationController navigationController)
    {
        this.navigationController = navigationController ?? throw new ArgumentNullException(nameof(navigationController));
    }

    public void NavigateToPage2() => this.navigationController.NavigateToPage2("Page 1");

    protected override void OnActivate()
    {
        base.OnActivate();
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
    }
}