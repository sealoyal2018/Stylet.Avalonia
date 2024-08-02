using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Stylet.Avalonia;

/// <summary>
/// StyletApplication to be extended by applications which don't want to use StyletIoC as the IoC container.
/// </summary>
public abstract class StyletApplicationBase<T> : Application, IWindowManagerConfig, IDisposable
    where T: class
{
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
    public virtual AvaloniaObject? GetActiveWindow()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desk)
        {
            var win = desk.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            return win ?? desk.MainWindow;
        }

        if (ApplicationLifetime is ISingleViewApplicationLifetime single)
        {
            return single.MainView;
        }
        return null;
    }

    public sealed override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new NotImplementedException("Mobile terminal adaptation is not implemented"); // 移动端暂未支持
        }
        OnStart();
        var vm = IoC.Get<T>();
        var winmgr = IoC.Get<IWindowManager>();
        winmgr.ShowWindow(vm);
        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose()
    {
    }
}
