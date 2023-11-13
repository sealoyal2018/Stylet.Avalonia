using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet.Avalonia;
using Stylet.Avalonia.Primitive;

namespace Stylet.Samples.Hello;

public class MainViewModel : Screen {
    private string _name;
    private readonly IWindowManager windowManager;
    
    public string Name {
        
        get
        {
            return this._name;
        }
        
        set
        {
            SetAndNotify(ref this._name, value);
            NotifyOfPropertyChange(nameof(CanSayHello));
        }
    }

    public MainViewModel(IWindowManager windowManager)
    {
        this.DisplayName = "Hello, Stylet";
        this.windowManager = windowManager;
    }

    public bool CanSayHello {
        get { return !String.IsNullOrEmpty(Name); }
    }
    
    public async Task SayHello()
    {
        await this.windowManager.ShowMessageBox<bool>(
            $"Hello, {this.Name}", 
            "提示框",
            MessageBoxButton.OK,
            MessageBoxImage.Information,
            MessageBoxResult.OK,
            MessageBoxResult.None
            );
        // DisplayName = String.Format("Hello, {0}", this.Name);
    }
}
