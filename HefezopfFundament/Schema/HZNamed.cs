namespace Hefezopf.Fundament.Schema {
    using Gsaelzbrot.Library;
    using System.Collections.Generic;

    /// <summary>
    /// Basetype with a name.
    /// </summary>
    public class HZNamed
        : INamed
        , IGsbNamed {
        /// <summary>
        /// Initializes a new instance of the <see cref="HZNamed"/> class.
        /// </summary>
        public HZNamed() { }

        /// <summary>
        /// Gets or sets the name,
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Utility for setting list properties.
        /// </summary>
        /// <typeparam name="T">The item Type.</typeparam>
        /// <param name="list">The target list.</param>
        /// <param name="value">The source list.</param>
        /// <returns>true if modified.</returns>
        public static bool SetterListProperty<T>(ICollection<T> list, ICollection<T> value) {
            if (ReferenceEquals(list, value)) { return false; }
            list.Clear();
            if (ReferenceEquals(null, value)) { return false; }
            bool result = false;
            foreach (var item in value) {
                list.Add(item);
                result = true;
            }
            return result;
        }
    }
}
