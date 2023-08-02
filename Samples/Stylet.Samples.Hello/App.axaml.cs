using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;

namespace Stylet.Samples.Hello {
    public partial class App : StyletApplication<MainViewModel> {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            //if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            //    desktop.MainWindow = new MainView();
            //}
            //var view = new MainView();
            base.OnFrameworkInitializationCompleted();
        }
    }
}