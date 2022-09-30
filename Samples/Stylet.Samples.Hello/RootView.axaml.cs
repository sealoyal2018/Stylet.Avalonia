using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Stylet.Samples.Hello;

public partial class RootView : Window
{
    public RootView()
    {
        //InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public void InitializeComponent1()
    {
        AvaloniaXamlLoader.Load(this);
    }
}