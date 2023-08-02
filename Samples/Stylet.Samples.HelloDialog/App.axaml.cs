using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using StyletIoC;

namespace Stylet.Samples.HelloDialog;
public partial class App : StyletApplication<ShellViewModel> {
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        //if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        //{
        //    desktop.MainWindow = new MainWindow();
        //}

        base.OnFrameworkInitializationCompleted();
    }

    protected override void ConfigureIoC(IStyletIoCBuilder builder)
    {

        base.ConfigureIoC(builder);
        builder.Bind<IDialogFactory>().ToAbstractFactory();

    }

}
