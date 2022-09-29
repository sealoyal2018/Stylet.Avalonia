using System.ComponentModel;

namespace Avalonia.Stylet;

/// <summary>
/// WeakEventManager for INotifyPropertyChanged.PropertyChanged.
/// </summary>
internal sealed class PropertyChangedWeakEventManager : 
    WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>
{
    protected override void StartListening(INotifyPropertyChanged source) => source.PropertyChanged += DeliverEvent;

    protected override void StopListening(INotifyPropertyChanged source) => source.PropertyChanged -= DeliverEvent;
}