using System.Reflection;
using Avalonia.Controls;

using DryIoc;
using Stylet.Avalonia.Primitive;

namespace Stylet.Avalonia.DryIoC;
public abstract class StyletApplication<T> : StyletApplicationBase<T> where T: class
{
    /// <summary>
    /// Gets or sets the StyletApplication's IoC container. This is created after ConfigureIoC has been run.
    /// </summary>
    private readonly IContainer _container;
    private readonly List<Assembly> _assemblies;
    
    protected StyletApplication()
    {
        _container = new Container();
        _assemblies = new List<Assembly>();
    }

    /// <summary>
    /// Overridden from StyletApplicationBase, this sets up the IoC container
    /// </summary>
    protected sealed override void Configure()
    {
        var assemblies = this.LoadAssemblies();
        this._assemblies.AddRange(assemblies);
        this.ConfigureIoC(_container);
        this.AutoRegister();
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
        return this._container.Resolve(type);
    }

    protected override object GetInstance(Type service, string? key)
    {
        return this._container.Resolve(service, serviceKey: key);
    }

    protected override IEnumerable<object> GetInstances(Type service)
    {
        return this._container.ResolveMany(service);
    }

    protected virtual void ConfigureIoC(IContainer container)
    {
        // Mark these as weak-bindings, so the user can replace them if they want
        var viewManagerConfig = new ViewManagerConfig()
        {
            ViewFactory = this.GetInstance,
            ViewAssemblies = new List<Assembly> { this.GetType().Assembly }
        };
        container.RegisterInstance(viewManagerConfig);
        container.Register<IViewManager, ViewManager>();
        container.RegisterInstance<IWindowManagerConfig>(this);
        container.Register<IWindowManager, WindowManager>();
        container.Register<IEventAggregator, EventAggregator>();
        container.Register<IMessageBoxViewModel, MessageBoxViewModel>();
        container.Register<MessageBoxView>();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();

        if (this._container != null)
            this._container.Dispose();
    }

    private void AutoRegister()
    {
        var viewManager = _container.Resolve(typeof(IViewManager)) as ViewManager;
        if (viewManager == null)
            throw new KeyNotFoundException($"{nameof(ViewManager)}未找到");
        var viewTypes = this._assemblies.SelectMany(v => v.GetTypes()).Where(v => v.FullName.EndsWith(viewManager.ViewModelNameSuffix) || typeof(Control).IsAssignableFrom(v)).ToList();
        foreach (var type in viewTypes)
        {
            _container.Register(type, type);
        }
    }
}