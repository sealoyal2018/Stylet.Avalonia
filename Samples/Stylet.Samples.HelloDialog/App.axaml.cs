using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using StyletIoC;

namespace Stylet.Samples.HelloDialog;
public partial class App : StyletIoCApplicationBase<ShellViewModel> {
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
