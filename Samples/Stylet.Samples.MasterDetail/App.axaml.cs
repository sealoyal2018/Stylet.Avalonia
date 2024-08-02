using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Stylet.Avalonia;
using Stylet.Avalonia.StyletIoC;

namespace Stylet.Samples.MasterDetail;
public partial class App : StyletApplication<ShellViewModel>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }
}
