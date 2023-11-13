using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;

namespace Stylet.Samples.Hello {
    public partial class App : StyletIoCApplicationBase<MainViewModel> {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }
    }
}