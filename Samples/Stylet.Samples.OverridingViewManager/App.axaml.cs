using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using StyletIoC;

namespace Stylet.Samples.OverridingViewManager
{
    public partial class App : StyletIoCApplicationBase<ShellViewModel>
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.Bind<IViewManager>().To<CustomViewManager>();
        }
    }
}