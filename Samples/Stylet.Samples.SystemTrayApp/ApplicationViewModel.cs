using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Stylet.Avalonia;
using Stylet.Avalonia.Logging;

namespace Stylet.Samples.SystemTrayApp;

public class ApplicationViewModel : PropertyChangedBase
{
    private IWindowManager _windowManager;
    private Func<MainViewModel> _mainViewModelFactory;
    private MainViewModel _mainViewModel;
    private string _instanceId;

    public ApplicationViewModel(
        IWindowManager windowManager,
        Func<MainViewModel> mainViewModelFactory)
    {
        InstanceId = Guid.NewGuid().ToString("D");
        
        _windowManager = windowManager;
        _mainViewModelFactory = mainViewModelFactory;
    }

    public string InstanceId
    {
        get => _instanceId;
        private set => SetAndNotify(ref _instanceId, value);
    }

    public bool CanShowMainWindow => true;

    public bool CanExitApplication => true;

    public void ShowMainWindow()
    {
        if (_mainViewModel == null)
            _mainViewModel = _mainViewModelFactory();

        _windowManager.ShowWindow(_mainViewModel!);
    }


    public void ExitApplication()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
}