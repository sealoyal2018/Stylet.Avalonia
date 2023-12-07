using Avalonia.Markup.Xaml;

using Stylet.Avalonia.DryIoC;

namespace Stylet.Samples.Hello
{
    public partial class App : StyletApplication<MainViewModel> {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }
    }
}