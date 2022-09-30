using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Stylet.Samples.TabNavigation;

public partial class Page1View : UserControl
{
    public Page1View()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}