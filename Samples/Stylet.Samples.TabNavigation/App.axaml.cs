using Avalonia.Markup.Xaml;
using Stylet.Avalonia.StyletIoC;

namespace Stylet.Samples.TabNavigation;

public partial class App : StyletApplication<ShellViewModel>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }
}