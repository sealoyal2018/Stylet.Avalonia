using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Stylet.Samples.NavigationController.Pages;
using StyletIoC;
using System;

namespace Stylet.Samples.NavigationController
{
    public partial class App : StyletIoCApplicationBase<ShellViewModel>
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.Bind<NavigationController>().And<INavigationController>().To<NavigationController>().InSingletonScope();
            // https://github.com/canton7/Stylet/issues/24
            builder.Bind<Func<Page1ViewModel>>().ToFactory<Func<Page1ViewModel>>(c => () => c.Get<Page1ViewModel>());
            builder.Bind<Func<Page2ViewModel>>().ToFactory<Func<Page2ViewModel>>(c => () => c.Get<Page2ViewModel>());
        }

        // protected override void OnLaunch()
        // {
        //     // There's a circular dependency, where ShellViewModel -> HeaderViewModel -> NavigationController -> ShellViewModel
        //     // We break this by assigning the ShellViewModel to the NavigationController after constructing it
        //     var navigationController = this.Container.Get<NavigationController>();
        //     navigationController.Delegate = this.RootViewModel;
        //     navigationController.NavigateToPage1();
        // }
    }
}