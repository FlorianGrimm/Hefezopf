using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema
{
    public interface INamed
    {
        string Name { get; }
    }

    public interface IHZDBOwned<in O>
    {
        void SetOwner(O owner);
    }

    public class HZDBOwnedCollection<T, O>
        : System.Collections.ObjectModel.ObservableCollection<T>
        where T : IHZDBOwned<O>
    {
        private readonly O _Owner;

        public HZDBOwnedCollection(O owner)
        {
            this._Owner = owner;
            this.CollectionChanged += this.OwnedCollection_CollectionChanged;
        }

        private void OwnedCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    this.SetOwner(e.NewItems);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    this.SetOwner(e.NewItems);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    this.SetOwner(e.NewItems);
                    break;
                default:
                    break;
            }
        }

        private void SetOwner(IList newItems)
        {
            if (newItems == null) { return; }
            foreach (T item in newItems)
            {
                item.SetOwner(this._Owner);
            }
        }
    }
}
