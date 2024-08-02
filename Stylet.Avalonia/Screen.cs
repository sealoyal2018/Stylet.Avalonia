using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia.Controls;
using Stylet.Avalonia.Logging;

namespace Stylet.Avalonia;

/// <summary>
/// Implementation of IScreen. Useful as a base class for your ViewModels
/// </summary>
public class Screen : ValidatingModelBase, IScreen
{
    private readonly ILogger logger;

    /// <summary>
    /// Initialises a new instance of the <see cref="Screen"/> class, without setting up a validator
    /// </summary>
    public Screen() : this(null) { }

    /// <summary>
    /// Initialises a new instance of the <see cref="Screen"/> class, which can validate properties using the given validator
    /// </summary>
    /// <param name="validator">Validator to use</param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Can be safely called from the Ctor, as it doesn't depend on state being set")]
    public Screen(IModelValidator validator) : base(validator)
    {
        var type = GetType();
        DisplayName = type.FullName;
        logger = LogManager.GetLogger(type);
    }

    #region IHaveDisplayName

    private string _displayName;

    /// <summary>
    /// Gets or sets the name associated with this ViewModel.
    /// Shown e.g. in a window's title bar, or as a tab's displayName
    /// </summary>
    public string DisplayName
    {
        get { return _displayName; }
        set { SetAndNotify(ref _displayName, value); }
    }

    #endregion

    #region IScreenState

    private ScreenState _screenState = ScreenState.Deactivated;

    /// <summary>
    /// Gets or sets the current state of the Screen
    /// </summary>
    [Obsolete("State is deprecated, please use ScreenState instead")]
    public virtual ScreenState State
    {
        get { return ScreenState; }
        protected set { ScreenState = value; }
    }

    /// <summary>
    /// Gets or sets the current state of the Screen
    /// </summary>
    public virtual ScreenState ScreenState
    {
        get { return _screenState; }
        protected set
        {
            if (SetAndNotify(ref _screenState, value))
            {
                // Temporary, until we remove 'State'
#pragma warning disable CS0618 // Type or member is obsolete
                NotifyOfPropertyChange("State");
#pragma warning restore CS0618 // Type or member is obsolete
            }
            NotifyOfPropertyChange("IsActive");
        }
    }

    /// <summary>
    /// Gets a value indicating whether the current state is ScreenState.Active
    /// </summary>
    public bool IsActive
    {
        get { return ScreenState == ScreenState.Active; }
    }

    private bool haveActivated = false;

    /// <summary>
    /// Raised when the Screen's state changed, for any reason
    /// </summary>
    public event EventHandler<ScreenStateChangedEventArgs> StateChanged;

    /// <summary>
    /// Fired whenever the Screen is activated
    /// </summary>
    public event EventHandler<ActivationEventArgs> Activated;

    /// <summary>
    /// Fired whenever the Screen is deactivated
    /// </summary>
    public event EventHandler<DeactivationEventArgs> Deactivated;

    /// <summary>
    /// Called whenever this Screen is closed
    /// </summary>
    public event EventHandler<CloseEventArgs> Closed;

    /// <summary>
    /// Called the very first time this Screen is activated, and never again
    /// </summary>
    protected virtual void OnInitialActivate() { }

    /// <summary>
    /// Called every time this screen is activated
    /// </summary>
    protected virtual void OnActivate() { }

    /// <summary>
    /// Called every time this screen is deactivated
    /// </summary>
    protected virtual void OnDeactivate() { }

    /// <summary>
    /// Called when this screen is closed
    /// </summary>
    protected virtual void OnClose() { }

    /// <summary>
    /// Called on any state transition
    /// </summary>
    /// <param name="previousState">Previous state state</param>
    /// <param name="newState">New state</param>
    protected virtual void OnStateChanged(ScreenState previousState, ScreenState newState) { }

    /// <summary>
    /// Sets the screen's state to the given state, if it differs from the current state
    /// </summary>
    /// <param name="newState">State to transition to</param>
    /// <param name="changedHandler">Called if the transition occurs. Arguments are (newState, previousState)</param>
    protected virtual void SetState(ScreenState newState, Action<ScreenState, ScreenState> changedHandler)
    {
        if (newState == ScreenState)
            return;

        var previousState = ScreenState;
        ScreenState = newState;

        logger.Info("Setting state from {0} to {1}", previousState, newState);

        OnStateChanged(previousState, newState);
        changedHandler(previousState, newState);

        var handler = StateChanged;
        if (handler != null)
            Execute.OnUIThread(() => handler(this, new ScreenStateChangedEventArgs(newState, previousState)));
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "As this is a framework type, don't want to make it too easy for users to call this method")]
    void IScreenState.Activate()
    {
        SetState(ScreenState.Active, (oldState, newState) =>
        {
            bool isInitialActivate = !haveActivated;
            if (!haveActivated)
            {
                OnInitialActivate();
                haveActivated = true;
            }

            OnActivate();

            var handler = Activated;
            if (handler != null)
                Execute.OnUIThread(() => handler(this, new ActivationEventArgs(oldState, isInitialActivate)));
        });
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "As this is a framework type, don't want to make it too easy for users to call this method")]
    void IScreenState.Deactivate()
    {
        // Avoid going from Closed -> Deactivated without going via Activated
        if (ScreenState == ScreenState.Closed)
            ((IScreenState)this).Activate();

        SetState(ScreenState.Deactivated, (oldState, newState) =>
        {
            OnDeactivate();

            var handler = Deactivated;
            if (handler != null)
                Execute.OnUIThread(() => handler(this, new DeactivationEventArgs(oldState)));
        });
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "As this is a framework type, don't want to make it too easy for users to call this method")]
    void IScreenState.Close()
    {
        // Avoid going from Activated -> Closed without going via Deactivated
        if (ScreenState != ScreenState.Closed)
            ((IScreenState)this).Deactivate();

        View = null;
        // Reset, so we can initially activate again
        haveActivated = false;

        SetState(ScreenState.Closed, (oldState, newState) =>
        {
            OnClose();

            var handler = Closed;
            if (handler != null)
                Execute.OnUIThread(() => handler(this, new CloseEventArgs(oldState)));
        });
    }

    #endregion

    #region IViewAware

    /// <summary>
    /// Gets the View attached to this ViewModel, if any. Using this should be a last resort
    /// </summary>
    public Control View { get; private set; }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "As this is a framework type, don't want to make it too easy for users to call this method")]
    void IViewAware.AttachView(Control view)
    {
        if (View != null)
            throw new InvalidOperationException(string.Format("Tried to attach View {0} to ViewModel {1}, but it already has a view attached", view.GetType().Name, GetType().Name));

        View = view;

        logger.Info("Attaching view {0}", view);

        var viewAsFrameworkElement = view as Control;
        if (viewAsFrameworkElement != null)
        {
            if (viewAsFrameworkElement.IsLoaded)
                OnViewLoaded();
            else
                viewAsFrameworkElement.Loaded += (o, e) => OnViewLoaded();
        }
    }

    /// <summary>
    /// Called when the view attaches to the Screen loads
    /// </summary>
    protected virtual void OnViewLoaded() { }

    #endregion

    #region IChild

    private object _parent;

    /// <summary>
    /// Gets or sets the parent conductor of this screen. Used to RequestClose to request a closure
    /// </summary>
    public object Parent
    {
        get { return _parent; }
        set { SetAndNotify(ref _parent, value); }
    }

    #endregion

    #region IGuardClose

    /// <summary>
    /// Called when a conductor wants to know whether this screen can close.
    /// </summary>
    /// <remarks>Internally, this calls CanClose, and wraps the response in a Task</remarks>
    /// <returns>A task returning true (can close) or false (can't close)</returns>
    public virtual Task<bool> CanCloseAsync()
    {
        // Temporary, before we remove CanClose()
#pragma warning disable CS0618 // Type or member is obsolete
        return Task.FromResult(CanClose());
#pragma warning restore CS0618 // Type or member is obsolete
    }

    /// <summary>
    /// Synchronous alternative to CanClose
    /// </summary>
    /// <returns>True if this screen can close, or false otherwise</returns>
    [Obsolete("This method is deprecated, please use CanCloseAsync() instead")]
    protected virtual bool CanClose()
    {
        return true;
    }

    #endregion

    #region IRequestClose

    /// <summary>
    /// Request that the conductor responsible for this screen close it
    /// </summary>
    /// <param name="dialogResult">DialogResult to return, if this is a dialog</param>
    public virtual void RequestClose(bool? dialogResult = null)
    {
        var conductor = Parent as IChildDelegate;
        if (conductor != null)
        {
            logger.Info("RequstClose called. Conductor: {0}; DialogResult: {1}", conductor, dialogResult);
            conductor.CloseItem(this, dialogResult);
        }
        else
        {
            var e = new InvalidOperationException(string.Format("Unable to close ViewModel {0} as it must have a conductor as a parent (note that windows and dialogs automatically have such a parent)", GetType()));
            logger.Error(e);
            throw e;
        }
    }

    #endregion

    /// <summary>
    /// Obselete - use RequestClose
    /// </summary>
    /// <param name="dialogResult">DialogResult to return, if this is a dialog</param>
    [Obsolete("Obseleted by RequestClose")]
    public virtual void TryClose(bool? dialogResult = null)
    {
        RequestClose(dialogResult);
    }
}
