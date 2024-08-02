using System.ComponentModel;

namespace Stylet.Avalonia;

/// <summary>
/// WeakEventManager for INotifyPropertyChanged.PropertyChanged.
/// </summary>
internal sealed class PropertyChangedWeakEventManager :
    WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>
{
    protected override void StartListening(INotifyPropertyChanged source) => source.PropertyChanged += DeliverEvent;

    protected override void StopListening(INotifyPropertyChanged source) => source.PropertyChanged -= DeliverEvent;
}