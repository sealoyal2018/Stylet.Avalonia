using Stylet.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stylet.Samples.HelloDialog;
public class Dialog1ViewModel : Screen
{
    public string Name { get; set; }

    public Dialog1ViewModel()
    {
        this.DisplayName = "I'm Dialog 1";
    }

    public void Close()
    {
        this.RequestClose(null);
    }

    public void Save()
    {
        this.RequestClose(true);
    }
}
