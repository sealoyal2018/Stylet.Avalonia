using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stylet.Avalonia;
using Stylet.Avalonia.StyletIoC;

namespace Stylet.Samples.SystemTrayApp;

public partial class App : StyletIoCApplication<MainViewModel>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }

    protected override void ConfigureLaunch(IStyletAppLaunchConfigBuilder builder)
    {
        base.ConfigureLaunch(builder);
        builder.UseApplicationViewModel<ApplicationViewModel>()
            .UseRootWindowViewModel<MainViewModel>(RootWindowStartupMode.NotShow)
            .WithDesktopShutdownMode(ShutdownMode.OnExplicitShutdown);
    }
}