namespace Hefezopf.Fundament.Schema {
    /// <summary>
    /// A object that is owned.
    /// </summary>
    /// <typeparam name="O">The type of the owner.</typeparam>
    public interface IHZDBOwned<in O> {
        /// <summary>
        /// Sets the owner in this.
        /// </summary>
        /// <param name="owner">The owner.</param>
        void SetOwner(O owner);
    }

}
