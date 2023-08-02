using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;

namespace Stylet.Samples.Hello {
    public partial class App : StyletApplication<MainViewModel> {
        public override void Initialize()
        {
            Console.WriteLine("Initialize()");
            AvaloniaXamlLoader.Load(this);

            // Initializes Prism.Avalonia - DO NOT REMOVE
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