using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Stylet.Samples.NavigationController.Pages;
using System;
using Avalonia.Controls;
using Stylet.Avalonia;
using Stylet.Avalonia.StyletIoC;

namespace Stylet.Samples.NavigationController
{
    public partial class App : StyletApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.Bind<NavigationController>().And<INavigationController>().To<NavigationController>().InSingletonScope();
            builder.Bind<ShellViewModel>().And<INavigationControllerDelegate>().To<ShellViewModel>().InSingletonScope();
            // https://github.com/canton7/Stylet/issues/24
            builder.Bind<Func<Page1ViewModel>>().ToFactory<Func<Page1ViewModel>>(c => () => c.Get<Page1ViewModel>());
            builder.Bind<Func<Page2ViewModel>>().ToFactory<Func<Page2ViewModel>>(c => () => c.Get<Page2ViewModel>());
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