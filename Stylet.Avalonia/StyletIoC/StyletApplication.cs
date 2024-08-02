using System;
using System.Collections.Generic;
using System.Reflection;
using Stylet.Avalonia.Primitive;

namespace Stylet.Avalonia.StyletIoC;

/// <summary>
/// StyletApplication to be extended by any application which wants to use StyletIoC, but doesn't have a root ViewModel
/// </summary>
/// <remarks>
/// You would normally use <see cref="StyletApplication"/>, which lets you specify the root ViewModel
/// to display. If you don't want to show a window on startup, override <see cref="StyletApplicationBase"/>
/// but don't call <see cref="StyletApplicationBase.DisplayRootView()"/>. 
/// </remarks>
public abstract class StyletApplication<T> : StyletApplicationBase<T> where T : class
{
    /// <summary>
    /// Gets or sets the StyletApplication's IoC container. This is created after ConfigureIoC has been run.
    /// </summary>
    protected IContainer Container { get; private set; }

    /// <summary>
    /// Overridden from StyletApplicationBase, this sets up the IoC container
    /// </summary>
    protected sealed override void Configure()
    {
        var builder = new StyletIoCBuilder
        {
            Assemblies = new List<Assembly> { GetType().Assembly }
        };

        // Call DefaultConfigureIoC *after* ConfigureIoIC, so that they can customize builder.Assemblies
        ConfigureIoC(builder);
        Container = builder.BuildContainer();
    }

    /// <summary>
    /// Override to add your own types to the IoC container.
    /// </summary>
    /// <param name="builder">StyletIoC builder to use to configure the container</param>
    protected virtual void ConfigureIoC(IStyletIoCBuilder builder)
    {
        // Mark these as weak-bindings, so the user can replace them if they want
        var viewManagerConfig = new ViewManagerConfig()
        {
            ViewFactory = GetInstance,
            ViewAssemblies = new List<Assembly>{ GetType().Assembly}
        };
        builder.Bind<ViewManagerConfig>().ToInstance(viewManagerConfig).AsWeakBinding();

        // Bind it to both IViewManager and to itself, so that people can get it with Container.Get<ViewManager>()
        builder.Bind<IViewManager>().And<ViewManager>().To<ViewManager>().InSingletonScope().AsWeakBinding();

        builder.Bind<IWindowManagerConfig>().ToInstance(this).DisposeWithContainer(false).AsWeakBinding();
        builder.Bind<IWindowManager>().To<WindowManager>().InSingletonScope().AsWeakBinding();
        builder.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope().AsWeakBinding();
        builder.Bind<IMessageBoxViewModel>().To<MessageBoxViewModel>().AsWeakBinding();
        // Stylet's assembly isn't added to the container, so add this explicitly
        builder.Bind<MessageBoxView>().ToSelf();
        builder.Autobind(GetType().Assembly);
    }

    protected override object GetInstance(Type service, string? key)
    {
        return Container.Get(service);
    }

    protected override IEnumerable<object> GetInstances(Type service)
    {
        return Container.GetAll(service);
    }

    /// <summary>
    /// Given a type, use the IoC container to fetch an instance of it
    /// </summary>
    /// <param name="type">Type to fetch</param>
    /// <returns>Fetched instance</returns>
    protected override object GetInstance(Type type)
    {
        return Container.Get(type);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();

        // Dispose the container last
        if (Container != null)
            Container.Dispose();
    }
}