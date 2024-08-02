using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stylet.Avalonia;
using Stylet.Avalonia.Primitive;

namespace Stylet.Samples.MSIoC;

public class MsStyletApplication<T> : StyletApplicationBase<T>
where T: class
{
    private readonly IServiceCollection _serviceCollection;
    private IServiceProvider? serviceProvider;
    private readonly List<Assembly> _assemblies;
    
    protected MsStyletApplication()
    {
        _serviceCollection = new ServiceCollection();
        _assemblies = new List<Assembly>();
    }

    /// <summary>
    /// Overridden from StyletApplicationBase, this sets up the IoC container
    /// </summary>
    protected sealed override void Configure()
    {
        var assemblies = this.LoadAssemblies();
        this._assemblies.AddRange(assemblies);
        this.ConfigureIoC(_serviceCollection);
        this.serviceProvider = _serviceCollection.BuildServiceProvider();
    }

    protected virtual List<Assembly> LoadAssemblies()
    {
        var assembly = Assembly.GetAssembly(this.GetType());
        if (assembly is null)
            assembly = Assembly.GetExecutingAssembly();
        return new List<Assembly> { assembly };
    }

    protected override object GetInstance(Type type)
    {
        if (serviceProvider is null)
            throw new TypeInitializationException(this.GetType().FullName, null);
        return serviceProvider!.GetRequiredService(type);
    }

    protected override object GetInstance(Type serviceType, string? key)
    {
        if (serviceProvider is null)
            throw new TypeInitializationException(this.GetType().FullName, null);
        return serviceProvider.GetRequiredKeyedService(serviceType, key);
    }

    protected override IEnumerable<object> GetInstances(Type service)
    {
        if (serviceProvider is null)
            throw new TypeInitializationException(this.GetType().FullName, null);
        return this.serviceProvider.GetServices(service);
    }

    protected virtual void ConfigureIoC(IServiceCollection services)
    {
        // Mark these as weak-bindings, so the user can replace them if they want
        var viewManagerConfig = new ViewManagerConfig()
        {
            ViewFactory = this.GetInstance,
            ViewAssemblies = new List<Assembly> { this.GetType().Assembly }
        };
        services.TryAddSingleton(viewManagerConfig);
        services.TryAddSingleton<IViewManager,ViewManager>();
        services.TryAddSingleton<IWindowManagerConfig>(this);
        services.TryAddSingleton<IWindowManager, WindowManager>();
        services.TryAddSingleton<IEventAggregator, EventAggregator>();
        services.TryAddSingleton<IMessageBoxViewModel, MessageBoxViewModel>();
        services.TryAddSingleton<MessageBoxView>();
        
        // register view and viewmodel
        if (services.BuildServiceProvider().GetService<IViewManager>() is not ViewManager viewManager)
            throw new KeyNotFoundException($"{nameof(ViewManager)}未找到");
        var viewModelNameSuffix = viewManager.ViewModelNameSuffix;
        var viewTypes = this._assemblies
            .SelectMany(v => v.GetTypes())
            .Where(v => v.FullName != null && v.FullName.EndsWith(viewModelNameSuffix) || typeof(Control).IsAssignableFrom(v)).ToList();
        foreach (var type in viewTypes)
        {
            if (typeof(Control).IsAssignableFrom(type))
            {
                services.AddTransient(type);
            }
            else
            {
                services.TryAddSingleton(type);
            }
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();

        this.serviceProvider = null;
    }
}