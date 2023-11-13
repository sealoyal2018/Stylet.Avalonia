using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet.Avalonia;

namespace Stylet.Samples.Hello;
public class MainViewModel : Screen {
    private IWindowManager windowManager;

    private string _name;
    public string Name {
        get { return this._name; }
        set
        {
            SetAndNotify(ref this._name, value);
            this.NotifyOfPropertyChange(nameof(CanSayHello));
        }
    }

    public MainViewModel(IWindowManager windowManager)
    {
        this.DisplayName = "Hello, Stylet";
        this.windowManager = windowManager;
    }

    public bool CanSayHello {
        get { return !String.IsNullOrEmpty(this.Name); }
    }
    public void SayHello()
    {
        //this.windowManager.ShowMessageBox(String.Format("Hello, {0}", this.Name));
        var vm = IoC.Get<MainViewModel>();
        vm.DisplayName = "Hello, Stylet3333333333";
    }
}
