// MvxLinearLayout.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System.Collections;
using System.Collections.Specialized;
using Android.Content;
using Android.Util;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Attributes;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace Cirrious.MvvmCross.Binding.Droid.Views
{
    public class MvxLinearLayout
        : LinearLayout
          , IMvxWithChangeAdapter
    {
        public MvxLinearLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            var itemTemplateId = MvxListViewHelpers.ReadAttributeValue(context, attrs,
                                                                       MvxAndroidBindingResource.Instance
                                                                                                .ListViewStylableGroupId,
                                                                       MvxAndroidBindingResource.Instance
                                                                                                .ListItemTemplateId);
            Adapter = new MvxAdapterWithChangedEvent(context);
            Adapter.ItemTemplateId = itemTemplateId;
            Adapter.DataSetChanged += AdapterOnDataSetChanged;
            this.ChildViewRemoved += OnChildViewRemoved;
        }

        public void AdapterOnDataSetChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            this.UpdateDataSetFromChange(sender, eventArgs);
        }

        private void OnChildViewRemoved(object sender, ChildViewRemovedEventArgs childViewRemovedEventArgs)
        {
            var boundChild = childViewRemovedEventArgs.Child as IMvxBindingContextOwner;
            if (boundChild != null)
            {
                boundChild.ClearAllBindings();
            }
        }

        private MvxAdapterWithChangedEvent _adapter;

        public MvxAdapterWithChangedEvent Adapter
        {
            get { return _adapter; }
            set
            {
                var existing = _adapter;
                if (existing == value)
                {
                    return;
                }

                if (existing != null)
                {
                    existing.DataSetChanged -= AdapterOnDataSetChanged;
                    if (value != null)
                    {
                        value.ItemsSource = existing.ItemsSource;
                        value.ItemTemplateId = existing.ItemTemplateId;
                    }
                }

                _adapter = value;

                if (_adapter != null)
                {
                    _adapter.DataSetChanged += AdapterOnDataSetChanged;
                }

                if (_adapter == null)
                {
                    MvxBindingTrace.Warning(
                        "Setting Adapter to null is not recommended - you amy lose ItemsSource binding when doing this");
                }
            }
        }

        [MvxSetToNullAfterBinding]
        public IEnumerable ItemsSource
        {
            get { return Adapter.ItemsSource; }
            set { Adapter.ItemsSource = value; }
        }

        public int ItemTemplateId
        {
            get { return Adapter.ItemTemplateId; }
            set { Adapter.ItemTemplateId = value; }
        }
    }
}