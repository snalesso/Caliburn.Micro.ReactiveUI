﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;

namespace Caliburn.Micro.ReactiveUI
{
    public class ReactiveViewAware : ReactiveObjectEx, Caliburn.Micro.IViewAware
    {
        private readonly IDictionary<object, object> views;

        /// <summary>
        /// The default view context.
        /// </summary>
        public static readonly object DefaultContext = new object();

        /// <summary>
        /// The view chache for this instance.
        /// </summary>
        protected IDictionary<object, object> Views
        {
            get { return this.views; }
        }

        /// <summary>
        /// Creates an instance of <see cref="ReactiveViewAware"/>.
        /// </summary>
        public ReactiveViewAware()
        {
            this.views = new WeakValueDictionary<object, object>();
        }

        /// <summary>
        /// Raised when a view is attached.
        /// </summary>
        public event EventHandler<ViewAttachedEventArgs> ViewAttached = delegate { };

        void IViewAware.AttachView(object view, object context)
        {
            this.Views[context ?? DefaultContext] = view;

            var nonGeneratedView = PlatformProvider.Current.GetFirstNonGeneratedView(view);
            PlatformProvider.Current.ExecuteOnFirstLoad(nonGeneratedView, this.OnViewLoaded);
            this.OnViewAttached(nonGeneratedView, context);

            this.ViewAttached(this, new ViewAttachedEventArgs { View = nonGeneratedView, Context = context });

            if (!(this is IActivate activatable) || activatable.IsActive)
            {
                PlatformProvider.Current.ExecuteOnLayoutUpdated(nonGeneratedView, this.OnViewReady);
            }
            else
            {
                AttachViewReadyOnActivated(activatable, nonGeneratedView);
            }
        }

        private static void AttachViewReadyOnActivated(IActivate activatable, object nonGeneratedView)
        {
            var viewReference = new WeakReference(nonGeneratedView);

            void handler(object s, ActivationEventArgs e)
            {
                ((IActivate)s).Activated -= handler;
                var view = viewReference.Target;
                if (view != null)
                {
                    PlatformProvider.Current.ExecuteOnLayoutUpdated(view, ((ReactiveViewAware)s).OnViewReady);
                }
            }

            activatable.Activated += handler;
        }

        /// <summary>
        /// Called when a view is attached.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="context">The context in which the view appears.</param>
        protected virtual void OnViewAttached(object view, object context)
        {
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name = "view"></param>
        protected virtual void OnViewLoaded(object view)
        {
        }

        /// <summary>
        /// Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name = "view"></param>
        protected virtual void OnViewReady(object view)
        {
        }

        /// <summary>
        /// Gets a view previously attached to this instance.
        /// </summary>
        /// <param name = "context">The context denoting which view to retrieve.</param>
        /// <returns>The view.</returns>
        public virtual object GetView(object context = null)
        {
            this.Views.TryGetValue(context ?? DefaultContext, out var view);
            return view;
        }
    }
}
