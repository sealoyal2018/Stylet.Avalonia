using Stylet.Xaml;
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
    public abstract class StyletApplicationBase<TRootViewModel> : Application, IBootstrapper, IWindowManagerConfig, IDisposable
        where   TRootViewModel:class
    {
        private TRootViewModel? _rootViewModel;

        /// <summary>
        /// Gets the root ViewModel, creating it first if necessary
        /// </summary>
        protected virtual TRootViewModel RootViewModel
        {
            get
            {
                if (_rootViewModel is null)
                    _rootViewModel = IoC.Get<TRootViewModel>();
                if (_rootViewModel is null)
                    throw new Exception($"No registration for the type {typeof(TRootViewModel)}.");
                return _rootViewModel;
            }
        }
        protected StyletApplicationBase()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            Setup(this);
        }

        public void Setup(Application application)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            Execute.Dispatcher = new ApplicationDispatcher();
            this.OnStart();
            this.ConfigureBootstrapper();

            this.Configure();
            IoC.GetInstance = this.GetInstance;
            IoC.GetInstances = this.GetInstances;
        }

        protected abstract object GetInstance(Type service, string? key);
        protected abstract IEnumerable<object> GetInstances(Type service);
        
        /// <summary>
        /// Hook called after the IoC container has been set up
        /// </summary>
        protected virtual void Configure() { }

        /// <summary>
        /// Launch the root view
        /// </summary>
        protected virtual Control? DisplayRootView()
        {
            var viewManager = IoC.Get<IViewManager>();
            return viewManager.CreateAndBindViewForModelIfNecessary(RootViewModel);
        }

        /// <summary>
        /// Returns the currently-displayed window, or null if there is none (or it can't be determined)
        /// </summary>
        /// <returns>The currently-displayed window, or null</returns>
        public virtual AvaloniaObject? GetActiveWindow()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desk)
            {
                return desk.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? desk.MainWindow;
            }

            if (ApplicationLifetime is ISingleViewApplicationLifetime single)
            {
                return single.MainView;
            }
            return null;
        }

        /// <summary>
        /// Override to configure your IoC container, and anything else
        /// </summary>
        protected virtual void ConfigureBootstrapper() { }

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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if(this._rootViewModel is not null)
                ScreenExtensions.TryDispose(this._rootViewModel);
        }

        public sealed override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = DisplayRootView() as Window;
            }

            if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                singleView.MainView = DisplayRootView();
            }
            
            base.OnFrameworkInitializationCompleted();
        }
    }
}
