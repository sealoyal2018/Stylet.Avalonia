using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Stylet.Avalonia;
using Stylet.Avalonia.StyletIoC;

namespace Stylet.Samples.Hello;

public partial class App : StyletIoCApplication<MainViewModel>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }
}