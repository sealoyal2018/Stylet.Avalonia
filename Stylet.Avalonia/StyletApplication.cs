using Avalonia.Markup.Xaml;

namespace Stylet.Avalonia;

public abstract class StyletApplication<TRootViewModel> : StyletApplicationBase
    where TRootViewModel : class
{
    protected override void ConfigureLaunch(IStyletAppLaunchConfigBuilder builder)
    {
        builder.UseRootWindowViewModel<TRootViewModel>();
    }
}