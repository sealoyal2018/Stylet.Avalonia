using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Stylet.Samples.MasterDetail;
public partial class ShellView : Window
{
    public ShellView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
