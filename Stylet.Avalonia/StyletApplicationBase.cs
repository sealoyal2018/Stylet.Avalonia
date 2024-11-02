using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Stylet.Avalonia.Xaml;

namespace Stylet.Avalonia;

/// <summary>
/// StyletApplication to be extended by applications which don't want to use StyletIoC as the IoC container.
/// </summary>
public abstract class StyletApplicationBase : Application, IWindowManagerConfig, IDisposable
{
    private IStyletAppLaunchConfig? _launchConfig;

    public override void Initialize()
    {
        IoC.GetInstance = GetInstance;
        IoC.GetInstances = GetInstances;
        base.Initialize();
        Execute.Dispatcher = new ApplicationDispatcher();
        Configure();
    }
    protected abstract IEnumerable<object> GetInstances(Type service);

    /// <summary>
    /// Given a type, use the IoC container to fetch an instance of it
    /// </summary>
    /// <param name="type">Type of instance to fetch</param>
    /// <returns>Fetched instance</returns>
    protected abstract object GetInstance(Type type);

    protected abstract object GetInstance(Type service, string? key);
    
    protected abstract void ConfigureLaunch(IStyletAppLaunchConfigBuilder builder);

    /// <summary>
    /// Override to configure your IoC container, and anything else
    /// </summary>
    protected virtual void Configure() { }

    /// <summary>
    /// Called on application startup. This occur after this.Args has been assigned, but before the IoC container has been configured
    /// </summary>
    protected virtual void OnStart() { }
    
    /// <summary>
    /// Returns the currently-displayed window, or null if there is none (or it can't be determined)
    /// </summary>
    /// <returns>The currently-displayed window, or null</returns>
    public virtual TopLevel? GetActiveWindow()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desk)
        {
            // return win ?? desk.MainWindow;
            throw new NotImplementedException("Mobile terminal adaptation is not implemented"); // 移动端暂未支持
        }
        
        var win = desk.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
        return TopLevel.GetTopLevel(win);
    }
    
    /// <summary>
    /// Launch the root view
    /// </summary>
    protected virtual void DisplayRootView(object rootViewModel)
    {
        var windowManager = IoC.Get<IWindowManager>();
        windowManager.ShowWindow(rootViewModel);
    }

    public sealed override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new NotImplementedException("Mobile terminal adaptation is not implemented"); // 移动端暂未支持
        }

        _launchConfig = StyletAppLaunchConfigBuilder
            .Configure(ConfigureLaunch)
            .Build();
        
        if (_launchConfig.RootWindowViewModelType == null &&
            _launchConfig.ShutdownMode is ShutdownMode.OnMainWindowClose or ShutdownMode.OnLastWindowClose)
        {
            throw new InvalidOperationException("RootWindowViewModelType must be set if ShutdownMode is OnMainWindowClose or OnLastWindowClose");
        }

        OnStart();

        if (_launchConfig.RootWindowViewModelType != null && _launchConfig.RootWindowStartupMode == RootWindowStartupMode.Show)
        {
            var rootViewModel = GetInstance(_launchConfig.RootWindowViewModelType);
            DisplayRootView(rootViewModel);
        }
        
        if (_launchConfig.ApplicationViewModelType != null)
        {
            var applicationViewModel = GetInstance(_launchConfig.ApplicationViewModelType);

            // Allow the application view model to be the target of actions
            View.SetActionTarget(this, applicationViewModel);
            
            DataContext = applicationViewModel;
        }
        
        desktop.ShutdownMode = _launchConfig.ShutdownMode;

        base.OnFrameworkInitializationCompleted();
        
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose()
    {
    }
}