namespace Hefezopf.Fundament.Schema {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Wrapping list.
    /// </summary>
    /// <typeparam name="O">The item type outside.</typeparam>
    /// <typeparam name="I">The item type inside.</typeparam>
    public class HZCastingList<O, I>
        : IList<O> {
        private readonly IList<I> _InnerList;
        private readonly Func<O, I> _ConvertO2I;
        private readonly Func<I, O> _ConvertI2O;

        /// <summary>
        /// Initializes a new instance of the <see cref="HZCastingList{O, I}"/> class.
        /// </summary>
        /// <param name="innerList">the real list</param>
        /// <param name="convertO2I">convert outer to innner.</param>
        /// <param name="convertI2O">convert inner to outer.</param>
        public HZCastingList(IList<I> innerList, Func<O, I> convertO2I, Func<I, O> convertI2O) {
            this._InnerList = innerList;
            this._ConvertI2O = convertI2O;
            this._ConvertO2I = convertO2I;
        }

        /// <summary>
        ///  Gets the number of elements contained in the list.
        /// </summary>
        public int Count => this._InnerList.Count;

        /// <summary>
        /// Gets a value indicating whether the list is read-only.
        /// </summary>
        /// Summary:
        public bool IsReadOnly => this._InnerList.IsReadOnly;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public O this[int index] {
            get {
                return this._ConvertI2O(((IList<I>)this._InnerList)[index]);
            }

            set {
                ((IList<I>)this._InnerList)[index] = this._ConvertO2I(value);
            }
        }

        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="item">The object to add to the System.Collections.Generic.ICollection`1.</param>
        public void Add(O item) {
            this._InnerList.Add(this._ConvertO2I(item));
        }

        /// <summary>
        /// Removes all items from the System.Collections.Generic.ICollection`1.
        /// </summary>
        public void Clear() {
            this._InnerList.Clear();
        }

        /// <summary>
        /// Determines whether the list contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the list.</param>
        /// <returns>true if item is found in the list; otherwise, false.</returns>
        public bool Contains(O item) {
            return this._InnerList.Contains(this._ConvertO2I(item));
        }

        /// <summary>
        /// Copies the elements of the list to an System.Array
        /// starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the elements copied
        /// from list. The System.Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(O[] array, int arrayIndex) {
            for (int idx = 0; idx < this._InnerList.Count; idx++) {
                array[idx + arrayIndex] = this._ConvertI2O(this._InnerList[idx]);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<O> GetEnumerator() {
            return this._InnerList.Select(i => this._ConvertI2O(i)).GetEnumerator();
        }

        /// <summary>
        /// Determines the index of a specific item in the list.
        /// </summary>
        /// <param name="item">The object to locate in the list.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(O item) {
            return this._InnerList.IndexOf(this._ConvertO2I(item));
        }

        /// <summary>
        /// Inserts an item to the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the list.</param>
        public void Insert(int index, O item) {
            this._InnerList.Insert(index, this._ConvertO2I(item));
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the list.
        /// </summary>
        /// <param name="item">the item to delete</param>
        /// <returns>
        /// true if item was successfully removed from the list;
        /// otherwise, false. This method also returns false if item is not found in the
        /// original list.
        /// </returns>
        public bool Remove(O item) {
            return this._InnerList.Remove(this._ConvertO2I(item));
        }

        /// <summary>
        /// Removes the list item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index) {
            this._InnerList.RemoveAt(index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return this._InnerList.Select(i => this._ConvertI2O(i)).GetEnumerator();
        }
    }
}
