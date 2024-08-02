using Avalonia.Markup.Xaml;

namespace Stylet.Samples.DryIoC;

public partial class App : DryIocStyletApplication<MainViewModel>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }
}