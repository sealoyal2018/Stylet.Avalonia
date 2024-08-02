using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Stylet.Avalonia;
using Stylet.Avalonia.StyletIoC;

namespace Stylet.Samples.TabNavigation
{
    public partial class App : StyletApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }
        
        protected override Control DisplayRootView()
        {
            var viewManager = IoC.Get<IViewManager>();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainViewModel = IoC.Get<ShellViewModel>();
                return viewManager.CreateAndBindViewForModelIfNecessary(mainViewModel);
            }

            throw new NotSupportedException();
        }
    }
}