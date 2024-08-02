using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Stylet.Avalonia;

public partial class Conductor<T>
{
    public partial class Collection
    {
        /// <summary>
        /// Conductor with many items, only one of which is active
        /// </summary>
        public class OneActive : ConductorBaseWithActiveItem<T>
        {
            private readonly BindableCollection<T> items = new BindableCollection<T>();

            private List<T> itemsBeforeReset;

            /// <summary>
            /// Gets the tems owned by this Conductor, one of which is active
            /// </summary>
            public IObservableCollection<T> Items
            {
                get { return items; }
            }

            /// <summary>
            /// Initialises a new instance of the <see cref="Conductor{T}.Collection.OneActive"/> class
            /// </summary>
            public OneActive()
            {
                items.CollectionChanging += (o, e) =>
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Reset:
                            itemsBeforeReset = items.ToList();
                            break;
                    }
                };

                items.CollectionChanged += (o, e) =>
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            this.SetParentAndSetActive(e.NewItems, false);
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            // ActiveItemMayHaveBeenRemovedFromItems may deactivate the ActiveItem; CloseAndCleanUp may close it.
                            // Call the methods in this order to avoid closing then deactivating (which causes reactivation)
                            ActiveItemMayHaveBeenRemovedFromItems();
                            this.CloseAndCleanUp(e.OldItems, DisposeChildren);
                            break;

                        case NotifyCollectionChangedAction.Replace:
                            // ActiveItemMayHaveBeenRemovedFromItems may deactivate the ActiveItem; CloseAndCleanUp may close it.
                            // Call the methods in this order to avoid closing then deactivating (which causes reactivation)
                            ActiveItemMayHaveBeenRemovedFromItems();
                            this.CloseAndCleanUp(e.OldItems, DisposeChildren);
                            this.SetParentAndSetActive(e.NewItems, false);
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            // ActiveItemMayHaveBeenRemovedFromItems may deactivate the ActiveItem; CloseAndCleanUp may close it.
                            // Call the methods in this order to avoid closing then deactivating (which causes reactivation)
                            ActiveItemMayHaveBeenRemovedFromItems();
                            this.CloseAndCleanUp(itemsBeforeReset.Except(items), DisposeChildren);
                            this.SetParentAndSetActive(items.Except(itemsBeforeReset), false);
                            itemsBeforeReset = null;
                            break;
                    }
                };
            }

            /// <summary>
            /// Called when the ActiveItem may have been removed from the Items collection. If it has, will change the ActiveItem to something sensible
            /// </summary>
            protected virtual void ActiveItemMayHaveBeenRemovedFromItems()
            {
                if (items.Contains(ActiveItem))
                    return;

                // Only close the previous item if it's in this.items - if it isn't, we'll
                // have already have closed it as part of reacting to changes in this.items.
                ChangeActiveItem(items.FirstOrDefault(), items.Contains(ActiveItem));
            }

            /// <summary>
            /// Return all items associated with this conductor
            /// </summary>
            /// <returns>All children associated with this conductor</returns>
            public override IEnumerable<T> GetChildren()
            {
                return items;
            }

            /// <summary>
            /// Activate the given item and set it as the ActiveItem, deactivating the previous ActiveItem
            /// </summary>
            /// <param name="item">Item to deactivate</param>
            public override void ActivateItem(T item)
            {
                if (item != null && item.Equals(ActiveItem))
                {
                    if (IsActive)
                        ScreenExtensions.TryActivate(ActiveItem);
                }
                else
                {
                    ChangeActiveItem(item, false);
                }
            }

            /// <summary>
            /// Deactive the given item, and choose another item to set as the ActiveItem
            /// </summary>
            /// <param name="item">Item to deactivate</param>
            public override void DeactivateItem(T item)
            {
                if (item == null)
                    return;

                if (item.Equals(ActiveItem))
                {
                    var nextItem = DetermineNextItemToActivate(item);
                    ChangeActiveItem(nextItem, false);
                }
                else
                {
                    ScreenExtensions.TryDeactivate(item);
                }
            }

            /// <summary>
            /// Close the given item (if and when possible, depending on IGuardClose.CanCloseAsync). This will deactive if it is the active item
            /// </summary>
            /// <param name="item">Item to close</param>
            public override async void CloseItem(T item)
            {
                if (item == null || !await CanCloseItem(item))
                    return;

                if (item.Equals(ActiveItem))
                {
                    var nextItem = DetermineNextItemToActivate(item);
                    // Counter-intuitively, we *don't* want to close the old ActiveItem. Removing it from 'this.items' below
                    // will do that, and we don't want to do it twice.
                    ChangeActiveItem(nextItem, false);
                }
                // Likewise if it isn't the ActiveItem, don't call CloseAndCleanup, as removing from 'this.items' will do that

                items.Remove(item);
            }

            /// <summary>
            /// Given a list of items, and and item which is going to be removed, choose a new item to be the next ActiveItem 
            /// </summary>
            /// <param name="itemToRemove">Item to remove</param>
            /// <returns>The next item to activate, or default(T) if no such item exists</returns>
            protected virtual T DetermineNextItemToActivate(T itemToRemove)
            {
                if (itemToRemove == null)
                {
                    return items.FirstOrDefault();
                }
                else if (items.Count > 1)
                {
                    // indexOfItemBeingRemoved *can* be -1 - if the item being removed doesn't exist in the list
                    var indexOfItemBeingRemoved = items.IndexOf(itemToRemove);

                    if (indexOfItemBeingRemoved < 0)
                        return items[0];
                    else if (indexOfItemBeingRemoved == 0)
                        return items[1];
                    else
                        return items[indexOfItemBeingRemoved - 1];
                }
                else
                {
                    return default;
                }
            }

            /// <summary>
            /// Returns true if and when all children can close
            /// </summary>
            /// <returns>A task indicating whether this conductor can close</returns>
            public override Task<bool> CanCloseAsync()
            {
                // Temporarily, until we remove CanClose
#pragma warning disable CS0618 // Type or member is obsolete
                if (!CanClose())
#pragma warning restore CS0618 // Type or member is obsolete
                    return Task.FromResult(false);
                return CanAllItemsCloseAsync(items);
            }

            /// <summary>
            /// Ensures that all items are closed when this conductor is closed
            /// </summary>
            protected override void OnClose()
            {
                // We've already been deactivated by this point
                // Clearing this.items causes all to be closed
                items.Clear();
            }

            /// <summary>
            /// Ensure an item is ready to be activated
            /// </summary>
            /// <param name="newItem">New item to ensure</param>
            protected override void EnsureItem(T newItem)
            {
                if (!items.Contains(newItem))
                    items.Add(newItem);

                base.EnsureItem(newItem);
            }
        }
    }
}
