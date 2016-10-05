using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema
{
    public class HZNamed : INamed
    {
        public HZNamed() { }

        public virtual string Name { get; set; }

        public static bool SetterListProperty<T>(ICollection<T> list, ICollection<T> value) {
            if (ReferenceEquals(list, value)) { return false; }
            list.Clear();
            if (ReferenceEquals(null, value)) { return false; }
            foreach (var item in value) {
                list.Add(item);
            }
            return true;
        }

        public static bool SetterListProperty<O, T>(ICollection<T> list, O owner, ICollection<T> value)
            where T : IHZDBOwned<O>
        {
            if (ReferenceEquals(list, value)) { return false; }
            list.Clear();
            if (ReferenceEquals(null, value)) { return false; }
            foreach (var item in value) {
                list.Add(item);
                item.SetOwner(owner);
            }
            return true;
        }
    }
}
