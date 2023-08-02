using StyletIoC;

namespace Stylet.Samples.OverridingViewManager;

public class AppBootstrapper:StyletApplication<ShellViewModel>
{
    protected override void ConfigureIoC(IStyletIoCBuilder builder)
    {
        builder.Bind<IViewManager>().To<CustomViewManager>();
    }
}