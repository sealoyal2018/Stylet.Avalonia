namespace Stylet.Avalonia;

public sealed class RelayCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute)
    {
        ArgumentNullException.ThrowIfNull(execute);
        this._execute = execute;
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public RelayCommand(Action execute, Func<bool> canExecute)
    {
        ArgumentNullException.ThrowIfNull(execute);
        ArgumentNullException.ThrowIfNull(canExecute);

        this._execute = execute;
        this._canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute is null || _canExecute.Invoke();
    }

    public void Execute(object? parameter)
    {
        this._execute.Invoke();
    }
}