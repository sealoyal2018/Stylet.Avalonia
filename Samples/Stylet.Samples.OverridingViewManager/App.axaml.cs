using Avalonia.Markup.Xaml;
using Stylet.Avalonia;
using Stylet.Avalonia.StyletIoC;

namespace Stylet.Samples.OverridingViewManager;

public partial class App : StyletApplication<ShellViewModel>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }
    protected override void ConfigureIoC(IStyletIoCBuilder builder)
    {
        builder.Bind<IViewManager>().To<CustomViewManager>();
        base.ConfigureIoC(builder);
    }
}