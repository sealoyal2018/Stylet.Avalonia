using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Stylet.Avalonia;

namespace Stylet
{
    /// <summary>
    /// StyletApplication to be extended by applications which don't want to use StyletIoC as the IoC container.
    /// </summary>
    public abstract class StyletApplicationBase : Application, IWindowManagerConfig, IDisposable
    {
        public override void Initialize()
        {
            IoC.GetInstance = this.GetInstance;
            IoC.GetInstances = this.GetInstances;
            base.Initialize();
            Execute.Dispatcher = new ApplicationDispatcher();
            this.Configure();
        }


        protected abstract object GetInstance(Type service, string? key);
        protected abstract IEnumerable<object> GetInstances(Type service);

        /// <summary>
        /// Override to configure your IoC container, and anything else
        /// </summary>
        protected virtual void Configure() { }

        /// <summary>
        /// Given a type, use the IoC container to fetch an instance of it
        /// </summary>
        /// <param name="type">Type of instance to fetch</param>
        /// <returns>Fetched instance</returns>
        protected abstract object GetInstance(Type type);

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
            this.OnStart();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var win = DisplayRootView() as Window;
                if (win is null)
                    throw new Exception($"{nameof(IClassicDesktopStyleApplicationLifetime)} 模式下应创建 window 类型作为主视图");
                //desktop.MainWindow = win;
                var wmgr = IoC.Get<IWindowManager>();
                wmgr.ShowWindow(win.DataContext);
            }

            if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                singleView.MainView = DisplayRootView();
            }
            
            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
        /// Launch the root view
        /// </summary>
        protected abstract Control DisplayRootView();


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
