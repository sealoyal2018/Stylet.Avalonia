using System.Collections.Generic;

namespace Stylet.Avalonia;

/// <summary>
/// Base class for all conductors which had a single active item
/// </summary>
/// <typeparam name="T">Type of item being conducted</typeparam>
public abstract class ConductorBaseWithActiveItem<T> : ConductorBase<T>, IHaveActiveItem<T> where T : class
{
    private T _activeItem;

    /// <summary>
    /// Gets or sets the item which is currently active
    /// </summary>
    public T ActiveItem
    {
        get { return _activeItem; }
        set { ActivateItem(value); }
    }

    /// <summary>
    /// From IParent, fetch all items
    /// </summary>
    /// <returns>Children of this conductor</returns>
    public override IEnumerable<T> GetChildren()
    {
        return new[] { ActiveItem };
    }

    /// <summary>
    /// Switch the active item to the given item
    /// </summary>
    /// <param name="newItem">New item to activate</param>
    /// <param name="closePrevious">Whether the previously-active item should be closed</param>
    protected virtual void ChangeActiveItem(T newItem, bool closePrevious)
    {
        ScreenExtensions.TryDeactivate(ActiveItem);
        if (closePrevious)
            this.CloseAndCleanUp(ActiveItem, DisposeChildren);

        _activeItem = newItem;

        if (newItem != null)
        {
            EnsureItem(newItem);

            if (IsActive)
                ScreenExtensions.TryActivate(newItem);
            else
                ScreenExtensions.TryDeactivate(newItem);
        }

        NotifyOfPropertyChange("ActiveItem");
    }

    /// <summary>
    /// When we're activated, also activate the ActiveItem
    /// </summary>
    protected override void OnActivate()
    {
        ScreenExtensions.TryActivate(ActiveItem);
    }

    /// <summary>
    /// When we're deactivated, also deactivate the ActiveItem
    /// </summary>
    protected override void OnDeactivate()
    {
        ScreenExtensions.TryDeactivate(ActiveItem);
    }

    /// <summary>
    /// When we're closed, also close the ActiveItem
    /// </summary>
    protected override void OnClose()
    {
        this.CloseAndCleanUp(ActiveItem, DisposeChildren);
    }
}
