using System;
using Avalonia.Controls;

namespace Stylet.Avalonia;

public enum RootWindowStartupMode
{
    Show,
    NotShow
}

public interface IStyletAppLaunchConfigBuilder
{
    IStyletAppLaunchConfigBuilder UseRootWindowViewModel<T>() where T : class;
    
    IStyletAppLaunchConfigBuilder UseRootWindowViewModel<T>(RootWindowStartupMode mode) where T : class;
    
    IStyletAppLaunchConfigBuilder UseApplicationViewModel<T>() where T : class;
    
    IStyletAppLaunchConfigBuilder WithDesktopShutdownMode(ShutdownMode mode);
}

public interface IStyletAppLaunchConfig
{
    Type? RootWindowViewModelType { get; }
    RootWindowStartupMode RootWindowStartupMode { get; }
    Type? ApplicationViewModelType { get; }
    ShutdownMode ShutdownMode { get; }
}

internal class StyletAppLaunchConfigBuilder : IStyletAppLaunchConfigBuilder
{
    private Type? _rootWindowViewModelType;
    private RootWindowStartupMode? _rootWindowStartupMode;
    private Type? _applicationViewModelType;
    private ShutdownMode? _shutdownMode;
    
    public static StyletAppLaunchConfigBuilder Configure(Action<IStyletAppLaunchConfigBuilder> configure)
    {
        var builder = new StyletAppLaunchConfigBuilder();
        configure(builder);
        return builder;
    }
    
    public IStyletAppLaunchConfigBuilder UseRootWindowViewModel<T>() where T : class
    {
        _rootWindowViewModelType = typeof(T);
        return this;
    }

    public IStyletAppLaunchConfigBuilder UseRootWindowViewModel<T>(RootWindowStartupMode mode) where T : class
    {
        _rootWindowViewModelType = typeof(T);
        _rootWindowStartupMode = mode;
        return this;
    }

    public IStyletAppLaunchConfigBuilder UseApplicationViewModel<T>() where T : class
    {
        _applicationViewModelType = typeof(T);
        return this;
    }

    public IStyletAppLaunchConfigBuilder WithDesktopShutdownMode(ShutdownMode mode)
    {
        _shutdownMode = mode;
        return this;
    }

    public IStyletAppLaunchConfig Build()
    {
        if (_rootWindowViewModelType == null &&
            _shutdownMode is ShutdownMode.OnMainWindowClose or ShutdownMode.OnLastWindowClose)
        {
            throw new InvalidOperationException("RootWindowViewModelType must be set if ShutdownMode is OnMainWindowClose or OnLastWindowClose");
        }
        
        return new StyletAppLaunchConfig
        {
            RootWindowViewModelType = _rootWindowViewModelType,
            RootWindowStartupMode = _rootWindowStartupMode ?? RootWindowStartupMode.Show,
            ApplicationViewModelType = _applicationViewModelType,
            ShutdownMode = _shutdownMode ?? ShutdownMode.OnMainWindowClose
        };
    }
    
    private record StyletAppLaunchConfig : IStyletAppLaunchConfig
    {
        public Type? RootWindowViewModelType { get; internal init; }
        public RootWindowStartupMode RootWindowStartupMode { get; internal init; }
        public Type? ApplicationViewModelType { get; internal init; }
        public ShutdownMode ShutdownMode { get; internal init;  }
    }
}