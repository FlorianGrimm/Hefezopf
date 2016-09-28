using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.DI
{
    /// <summary>
    /// a array of objects with a FAST GetHashCode and Equals - Implementation.
    /// two CacheKeys are equal when the items pairs are equal.
    /// the HashCode is the xor over the items HashCode.
    /// </summary>
    public sealed class CacheKey
    {
        /// <summary>
        /// Empty(Unit) CacheKey
        /// </summary>
        public readonly static CacheKey Empty = new CacheKey();
        //public static CacheKey CreateKey(CacheKey expand, params object[] parameterKey)
        //{
        //    if (expand == null)
        //    {
        //        return new CacheKey(parameterKey);
        //    }
        //    return new CacheKey(expand, parameterKey);
        //}
        public static CacheKey CreateKey(object parameterKey)
        {
            var result = parameterKey as CacheKey;
            if (result == null)
            {
                return new CacheKey(parameterKey);
            }
            else
            {
                return result;
            }
        }
        //
        readonly object[] _Keys;
        readonly int _HashCode;
        private CacheKey()
        {
            _Keys = new object[0];
            //_HashCode = 0;
        }
        public CacheKey(object key)
        {
            _Keys = new object[] { key ?? NullReplacement.NULL };
            if (object.ReferenceEquals(key, null))
            {
                _HashCode = 0;
            }
            else
            {
                _HashCode = key.GetHashCode();
            }
        }
        public CacheKey(params object[] values)
        {
            var l2 = values.Length;
            _Keys = values;
            if (l2 == 0)
            {
                _HashCode = 0;
            }
            else
            {
                int hc = 0;
                for (int i = 0; i < l2; i++)
                {
                    if (object.ReferenceEquals(values[i], null))
                    {
                        values[i] = NullReplacement.NULL;
                        //hc = hc ^ values[i].GetHashCode();
                        // ==> hc = hc ^ 0;
                        // ==> do nothing
                        // hc = ((hc & 0x0ffffff) << 4);
                        hc = (hc << 4);
                    }
                    else
                    {
                        //hc = ((hc & 0x0ffffff) << 4) ^ values[i].GetHashCode();
                        hc = (hc << 4) ^ values[i].GetHashCode();
                    }
                }
                _HashCode = hc;
            }
        }
        public CacheKey(CacheKey expand, params object[] values)
        {
            var l1 = (expand == null) ? 0 : expand._Keys.Length;
            var l2 = values.Length;
            if (l1 == 0)
            {
                _Keys = values;
                if (l2 == 0)
                {
                    _HashCode = 0;
                }
                else
                {
                    int hc = 0;
                    for (int i = 0; i < l2; i++)
                    {
                        if (object.ReferenceEquals(values[i], null))
                        {
                            values[i] = NullReplacement.NULL;
                            //hc = hc ^ values[i].GetHashCode();
                            // ==> hc = hc ^ 0;
                            // ==> do nothing
                            //hc = ((hc & 0x0ffffff) << 4);
                            hc = hc << 4;
                        }
                        else
                        {
                            //hc = ((hc & 0x0ffffff) << 4) ^ values[i].GetHashCode();
                            hc = (hc << 4) ^ values[i].GetHashCode();
                        }
                    }
                    _HashCode = hc;
                }
            }
            else
            {
                var len = l1 + l2;
                var keys = new object[len];
                Array.Copy(expand._Keys, keys, l1);
                Array.Copy(values, 0, keys, l1, l2);
                _Keys = keys;
                int hc = expand._HashCode;
                for (int i = l1; i < len; i++)
                {
                    if (object.Equals(keys[i], null))
                    {
                        keys[i] = NullReplacement.NULL;
                        //hc = hc ^ values[i].GetHashCode();
                        // ==> hc = hc ^ 0;
                        // ==> do nothing
                        //hc = ((hc & 0x0ffffff) << 4);
                        hc = (hc << 4);
                    }
                    else
                    {
                        //hc = ((hc & 0x0ffffff) << 4) ^ values[i].GetHashCode();
                        hc = (hc << 4) ^ keys[i].GetHashCode();
                    }
                }
                _HashCode = hc;
            }
        }
        public override int GetHashCode()
        {
            return _HashCode;
        }
        public override bool Equals(object obj)
        {
            // changing this code requiers special permissions from me - flori
            CacheKey other = obj as CacheKey;
            if (object.Equals(other, null)) { return false; }
            if (this._HashCode != other._HashCode) { return false; }
            var keysLength = this._Keys.Length;
            if (keysLength != other._Keys.Length) { return false; }
            for (int i = 0; i < keysLength; i++)
            {
                if (!this._Keys[i].Equals(other._Keys[i])) { return false; }
            }
            return true;
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < _Keys.Length; i++)
            {
                sb.Append(i).Append(":");
                var value = _Keys[i];
                if (object.Equals(value, null))
                {
                    sb.Append("NULL");
                }
                else
                {
                    sb.Append(value.ToString());
                }
                sb.Append(";");
            }
            return sb.ToString();
        }
        //
        public static CacheKey<T1> Create<T1>(T1 item1) { return new CacheKey<T1>(item1); }
        public static CacheKey<T1, T2> Create<T1, T2>(T1 item1, T2 item2) { return new CacheKey<T1, T2>(item1, item2); }
        public static CacheKey<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) { return new CacheKey<T1, T2, T3>(item1, item2, item3); }
        public static CacheKey<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) { return new CacheKey<T1, T2, T3, T4>(item1, item2, item3, item4); }
    }
    public sealed class CacheKey<T1>
    {
        T1 _Item1; int _HashCode;
        bool _Null1;
        public CacheKey(T1 item1)
        {
            if (object.Equals(item1, null))
            {
                _Null1 = true;
            }
            else
            {
                _Item1 = item1;
                _HashCode = _Item1.GetHashCode();
            }
        }
        public T1 Item1 { get { return _Item1; } }
        public override bool Equals(object obj)
        {
            CacheKey<T1> other = obj as CacheKey<T1>;
            if (object.Equals(other, null)) { return false; }
            if (this._HashCode != other._HashCode) { return false; }
            if (this._Null1 != other._Null1) { return false; }
            if (!this._Null1 && !other._Null1 && !this._Item1.Equals(other._Item1)) { return false; }
            return true;
        }
        public override int GetHashCode() { return _HashCode; }
    }
    public sealed class CacheKey<T1, T2>
    {
        T1 _Item1; T2 _Item2; int _HashCode;
        bool _Null1; bool _Null2;
        public CacheKey(T1 item1, T2 item2)
        {
            int hashCode = 0;
            if (object.Equals(item1, null))
            {
                _Null1 = true;
            }
            else
            {
                _Item1 = item1;
                hashCode = _Item1.GetHashCode();
            }
            if (object.Equals(item2, null))
            {
                _Null2 = true;
            }
            else
            {
                _Item2 = item2;
                hashCode = (hashCode << 4) ^ _Item2.GetHashCode();
            }
            _HashCode = hashCode;
        }
        public T1 Item1 { get { return _Item1; } }
        public T2 Item2 { get { return _Item2; } }
        public override bool Equals(object obj)
        {
            var other = obj as CacheKey<T1, T2>;
            if (object.Equals(other, null)) { return false; }
            if (this._HashCode != other._HashCode) { return false; }
            if (this._Null1 != other._Null1) { return false; }
            if (!this._Null1 && !other._Null1 && !this._Item1.Equals(other._Item1)) { return false; }
            if (this._Null2 != other._Null2) { return false; }
            if (!this._Null2 && !other._Null2 && !this._Item2.Equals(other._Item2)) { return false; }
            return true;
        }
        public override int GetHashCode() { return _HashCode; }
    }
    public sealed class CacheKey<T1, T2, T3>
    {
        T1 _Item1; T2 _Item2; T3 _Item3; int _HashCode;
        bool _Null1; bool _Null2; bool _Null3;
        public CacheKey(T1 item1, T2 item2, T3 item3)
        {
            int hashCode = 0;
            if (object.Equals(item1, null))
            {
                _Null1 = true;
            }
            else
            {
                _Item1 = item1;
                hashCode = _Item1.GetHashCode();
            }
            if (object.Equals(item2, null))
            {
                _Null2 = true;
            }
            else
            {
                _Item2 = item2;
                hashCode = (hashCode << 4) ^ _Item2.GetHashCode();
            }
            if (object.Equals(item3, null))
            {
                _Null3 = true;
            }
            else
            {
                _Item3 = item3;
                hashCode = (hashCode << 4) ^ _Item3.GetHashCode();
            }
            _HashCode = hashCode;
        }
        public T1 Item1 { get { return _Item1; } }
        public T2 Item2 { get { return _Item2; } }
        public T3 Item3 { get { return _Item3; } }
        public override bool Equals(object obj)
        {
            var other = obj as CacheKey<T1, T2, T3>;
            if (object.Equals(other, null)) { return false; }
            if (this._HashCode != other._HashCode) { return false; }
            if (this._Null1 != other._Null1) { return false; }
            if (!this._Null1 && !other._Null1 && !this._Item1.Equals(other._Item1)) { return false; }
            if (this._Null2 != other._Null2) { return false; }
            if (!this._Null2 && !other._Null2 && !this._Item2.Equals(other._Item2)) { return false; }
            if (this._Null3 != other._Null3) { return false; }
            if (!this._Null3 && !other._Null3 && !this._Item3.Equals(other._Item3)) { return false; }
            return true;
        }
        public override int GetHashCode() { return _HashCode; }
    }
    public sealed class CacheKey<T1, T2, T3, T4>
    {
        T1 _Item1; T2 _Item2; T3 _Item3; T4 _Item4; int _HashCode;
        bool _Null1; bool _Null2; bool _Null3; bool _Null4;
        public CacheKey(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            int hashCode = 0;
            if (object.Equals(item1, null))
            {
                _Null1 = true;
            }
            else
            {
                _Item1 = item1;
                hashCode = _Item1.GetHashCode();
            }
            if (object.Equals(item2, null))
            {
                _Null2 = true;
            }
            else
            {
                _Item2 = item2;
                hashCode = (hashCode << 4) ^ _Item2.GetHashCode();
            }
            if (object.Equals(item3, null))
            {
                _Null3 = true;
            }
            else
            {
                _Item3 = item3;
                hashCode = (hashCode << 4) ^ _Item3.GetHashCode();
            }
            if (object.Equals(item4, null))
            {
                _Null4 = true;
            }
            else
            {
                _Item4 = item4;
                hashCode = (hashCode << 4) ^ _Item4.GetHashCode();
            }
            _HashCode = hashCode;
        }
        public T1 Item1 { get { return _Item1; } }
        public T2 Item2 { get { return _Item2; } }
        public T3 Item3 { get { return _Item3; } }
        public T4 Item4 { get { return _Item4; } }
        public override bool Equals(object obj)
        {
            var other = obj as CacheKey<T1, T2, T3, T4>;
            if (object.Equals(other, null)) { return false; }
            if (this._HashCode != other._HashCode) { return false; }
            if (this._Null1 != other._Null1) { return false; }
            if (!this._Null1 && !other._Null1 && !this._Item1.Equals(other._Item1)) { return false; }
            if (this._Null2 != other._Null2) { return false; }
            if (!this._Null2 && !other._Null2 && !this._Item2.Equals(other._Item2)) { return false; }
            if (this._Null3 != other._Null3) { return false; }
            if (!this._Null3 && !other._Null3 && !this._Item3.Equals(other._Item3)) { return false; }
            if (this._Null4 != other._Null4) { return false; }
            if (!this._Null4 && !other._Null4 && !this._Item4.Equals(other._Item4)) { return false; }
            return true;
        }
        public override int GetHashCode() { return _HashCode; }
    }
}
