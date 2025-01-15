using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace Net.Axilog.Model.Base
{
    public class ThreadedBindingList<T> : BindingList<T>
    {

        public SynchronizationContext SynchronizationContext
        {
            get { return _ctx; }
            set { _ctx = value; }
        }

        public ThreadedBindingList(IList<T> _list):base(_list) 
    {
    }

        SynchronizationContext _ctx;
        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            if (_ctx == null)
            {
                BaseAddingNew(e);
            }
            else
            {
                SynchronizationContext.Current.Send(delegate
                {
                    BaseAddingNew(e);
                }, null);
            }
        }
        void BaseAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (_ctx == null)
            {
                BaseListChanged(e);
            }
            else
            {
                _ctx.Send(delegate { BaseListChanged(e); }, null);
            }
        }
        void BaseListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
        }
    }

    public class BindListWithRemoving<T> : BindingList<T> 
    { 
        
        public BindListWithRemoving(IList<T> list) : base(list) 
        { }

        protected override void RemoveItem(int index)
        {
            // NOTE: we could check if index is valid here before sending the event, this is arguable...
            OnListChanged(new ListChangedEventArgsWithRemovedItem<T>(this[index], index));

            // remove item without any duplicate event
            bool b = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            try
            {
                base.RemoveItem(index);
            }
            finally
            {
                RaiseListChangedEvents = b;
            }
        }
        

    
    }

    public class ListChangedEventArgsWithRemovedItem : ListChangedEventArgs
    {
        public ListChangedEventArgsWithRemovedItem(object item, int index)
            : base(ListChangedType.ItemDeleted, index, index)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Item = item;
        }

        public virtual object Item { get; protected set; }
    }

    public class ListChangedEventArgsWithRemovedItem<T> : ListChangedEventArgsWithRemovedItem
    {
        public ListChangedEventArgsWithRemovedItem(T item, int index)
            : base(item, index)
        {
        }

        public override object Item { get { return (T)base.Item; } protected set { base.Item = value; } }
    }

}
