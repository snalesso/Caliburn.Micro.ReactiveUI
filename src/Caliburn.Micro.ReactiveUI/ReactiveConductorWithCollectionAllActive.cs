﻿using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Caliburn.Micro.ReactiveUI
{
    public partial class ReactiveConductor<T>
    {
        /// <summary>
        /// An implementation of <see cref="IConductor"/> that holds on many items.
        /// </summary>
        public partial class Collection
        {
            /// <summary>
            /// An implementation of <see cref="IConductor"/> that holds on to many items wich are all activated.
            /// </summary>
            public class AllActive : ReactiveConductorBase<T>
            {
                readonly ObservableCollection<T> _items = new ObservableCollection<T>();
                readonly bool _openPublicItems;

                /// <summary>
                /// Initializes a new instance of the <see cref="Conductor&lt;T&gt;.Collection.AllActive"/> class.
                /// </summary>
                /// <param name="openPublicItems">if set to <c>true</c> opens public items that are properties of this class.</param>
                public AllActive(bool openPublicItems)
                    : this()
                {
                    this._openPublicItems = openPublicItems;
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="Conductor&lt;T&gt;.Collection.AllActive"/> class.
                /// </summary>
                public AllActive()
                {
                    this.Items = new ReadOnlyObservableCollection<T>(this._items);

                    this._items.CollectionChanged += (s, e) =>
                    {
                        switch (e.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                e.NewItems.OfType<IChild>().Apply(x => x.Parent = this);
                                break;
                            case NotifyCollectionChangedAction.Remove:
                                e.OldItems.OfType<IChild>().Apply(x => x.Parent = null);
                                break;
                            case NotifyCollectionChangedAction.Replace:
                                e.NewItems.OfType<IChild>().Apply(x => x.Parent = this);
                                e.OldItems.OfType<IChild>().Apply(x => x.Parent = null);
                                break;
                            case NotifyCollectionChangedAction.Reset:
                                this._items.OfType<IChild>().Apply(x => x.Parent = this);
                                break;
                        }
                    };
                }

                /// <summary>
                /// Gets the items that are currently being conducted.
                /// </summary>
                public ReadOnlyObservableCollection<T> Items { get; }

                /// <summary>
                /// Called when activating.
                /// </summary>
                protected override void OnActivate()
                {
                    this._items.OfType<IActivate>().Apply(x => x.Activate());
                }

                /// <summary>
                /// Called when deactivating.
                /// </summary>
                /// <param name="close">Inidicates whether this instance will be closed.</param>
                protected override void OnDeactivate(bool close)
                {
                    this._items.OfType<IDeactivate>().Apply(x => x.Deactivate(close));
                    if (close)
                    {
                        this._items.Clear();
                    }
                }

                /// <summary>
                /// Called to check whether or not this instance can close.
                /// </summary>
                /// <param name="callback">The implementor calls this action with the result of the close check.</param>
                public override void CanClose(Action<bool> callback)
                {
                    this.CloseStrategy.Execute(this.Items, (canClose, closeables) =>
                    {
                        if (!canClose && closeables.Any())
                        {
                            closeables.OfType<IDeactivate>().Apply(x => x.Deactivate(true));
                            foreach (var closeable in closeables)
                            {
                                this._items.Remove(closeable);
                            }
                        }

                        callback(canClose);
                    });
                }

                /// <summary>
                /// Called when initializing.
                /// </summary>
                protected override void OnInitialize()
                {
                    if (this._openPublicItems)
                    {
                        this.GetType().GetRuntimeProperties()
                            .Where(x => x.Name != "Parent" && typeof(T).GetTypeInfo().IsAssignableFrom(x.PropertyType.GetTypeInfo()))
                            .Select(x => x.GetValue(this, null))
                            .Cast<T>()
                            .Apply(this.ActivateItem);
                    }
                }

                /// <summary>
                /// Activates the specified item.
                /// </summary>
                /// <param name="item">The item to activate.</param>
                public override void ActivateItem(T item)
                {
                    if (item == null)
                    {
                        return;
                    }

                    item = this.EnsureItem(item);

                    if (this.IsActive)
                    {
                        ScreenExtensions.TryActivate(item);
                    }

                    this.OnActivationProcessed(item, true);
                }

                /// <summary>
                /// Deactivates the specified item.
                /// </summary>
                /// <param name="item">The item to close.</param>
                /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
                public override void DeactivateItem(T item, bool close)
                {
                    if (item == null)
                    {
                        return;
                    }

                    if (close)
                    {
                        this.CloseStrategy.Execute(new[] { item }, (canClose, closeable) =>
                        {
                            if (canClose)
                                this.CloseItemCore(item);
                        });
                    }
                    else
                    {
                        ScreenExtensions.TryDeactivate(item, false);
                    }
                }

                /// <summary>
                /// Gets the children.
                /// </summary>
                /// <returns>The collection of children.</returns>
                public override IEnumerable<T> GetChildren()
                {
                    return this._items;
                }

                void CloseItemCore(T item)
                {
                    ScreenExtensions.TryDeactivate(item, true);
                    this._items.Remove(item);
                }

                /// <summary>
                /// Ensures that an item is ready to be activated.
                /// </summary>
                /// <param name="newItem">The item that is about to be activated.</param>
                /// <returns>The item to be activated.</returns>
                protected override T EnsureItem(T newItem)
                {
                    var index = this._items.IndexOf(newItem);

                    if (index == -1)
                    {
                        this._items.Add(newItem);
                    }
                    else
                    {
                        newItem = this._items[index];
                    }

                    return base.EnsureItem(newItem);
                }
            }
        }
    }
}
