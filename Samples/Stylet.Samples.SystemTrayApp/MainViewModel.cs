using System;
using System.Threading.Tasks;
using Avalonia.Media;
using Stylet.Avalonia;
using Stylet.Avalonia.Primitive;

namespace Stylet.Samples.SystemTrayApp;

public class MainViewModel : Screen {
    private string _name;
    private readonly IWindowManager _windowManager;
    private string _instanceId;

    public MainViewModel(IWindowManager windowManager)
    {
        InstanceId = Guid.NewGuid().ToString("D");
        DisplayName = "Hello, Stylet";
        _windowManager = windowManager;
    }

    public string InstanceId
    {
        get => _instanceId;
        private set => SetAndNotify(ref _instanceId, value);
    }

    public string Name {
        
        get => _name;

        set
        {
            SetAndNotify(ref _name, value);
            NotifyOfPropertyChange(nameof(CanSayHello));
        }
    }

    public bool CanSayHello => !string.IsNullOrEmpty(Name);

    public async Task SayHello()
    {
        await _windowManager.ShowMessageBox<bool>(
            $"Hello, {Name}", 
            "Tip Box",
            MessageBoxButton.OKCancel,
            icon: MessageBoxImage.Information,
            textAlignment: TextAlignment.Center
            );
    }
}
