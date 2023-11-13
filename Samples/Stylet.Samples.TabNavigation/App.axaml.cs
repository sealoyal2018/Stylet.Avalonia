using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Stylet.Samples.TabNavigation
{
    public partial class App : StyletIoCApplicationBase<ShellViewModel>
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }
    }
}