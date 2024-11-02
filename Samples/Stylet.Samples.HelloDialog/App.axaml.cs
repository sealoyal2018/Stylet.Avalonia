using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Stylet.Avalonia;
using Stylet.Avalonia.StyletIoC;

namespace Stylet.Samples.HelloDialog;
public partial class App : StyletIoCApplication<ShellViewModel> {
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }
    
    protected override void ConfigureIoC(IStyletIoCBuilder builder)
    {
        base.ConfigureIoC(builder);
        builder.Bind<IDialogFactory>().ToAbstractFactory();
    }

}
