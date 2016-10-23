namespace Hefezopf.Fundament.Schema {
    using System.Collections;

    /// <summary>
    /// A collection for owned elements.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <typeparam name="O">The owner type.</typeparam>
    public class HZDBOwnedCollection<T, O>
        : System.Collections.ObjectModel.ObservableCollection<T>
        where T : IHZDBOwned<O> {
        private readonly O _Owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBOwnedCollection{T, O}"/> class.
        /// </summary>
        /// <param name="owner">The owner of this collection.</param>
        public HZDBOwnedCollection(O owner) {
            this._Owner = owner;
            this.CollectionChanged += this.OwnedCollection_CollectionChanged;
        }

        private void OwnedCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
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

        private void SetOwner(IList newItems) {
            if (newItems == null) { return; }
            foreach (T item in newItems) {
                item.SetOwner(this._Owner);
            }
        }
    }
}
