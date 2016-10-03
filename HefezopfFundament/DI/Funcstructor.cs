using Hefezopf.Contracts;
using Hefezopf.Contracts.DI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.DI
{
    public class Funcstructor
        : IDependencyInjection
        , IDependencyInjectionConfigurable
    {
        public const string ParentNameRoot = "Root";
        public const string ParentNameConfiguration = "Configuration";
        public const string ParentNameFallBack = "Funcstructor-FallBack";
        public static Funcstructor CreateDefault(IAssemblyLoaderService assemblyLoaderService)
        {
            var instanceFallback = new Funcstructor(ParentNameFallBack, null, null, false);
            var instanceConfiguration = new Funcstructor(ParentNameConfiguration, null, instanceFallback, false);
            var instanceRoot = new Funcstructor(ParentNameRoot, instanceConfiguration, instanceFallback, true);
            if ((object)assemblyLoaderService == null)
            {
                assemblyLoaderService.WireTo(instanceRoot);
            }
            return instanceRoot;
        }

        private ReaderWriterLockSlim _ReaderWriterLock;
        private Hashtable _Registry;
        public readonly string Name;
        private Funcstructor _Parent;
        private readonly bool _ContainsInstances;
        private Funcstructor _FallBack;

        public Funcstructor(string name, Funcstructor parent, Funcstructor fallBack)
        {
            //if (ParentNameFallBack.Equals(name)) { throw new ArgumentException("Funcstructor-FallBack is not allowed", "name"); }
            this._ReaderWriterLock = new ReaderWriterLockSlim();
            this._Registry = new Hashtable();
            this.Name = name ?? "";
            this._Parent = parent;
            this._FallBack = fallBack;
            this._ContainsInstances = true;
            //this._FallBack = new Funcstructor(ParentNameFallBack, null);
        }

        private Funcstructor(string name, Funcstructor parent, Funcstructor fallBack, bool containsInstances)
        {
            this._ReaderWriterLock = new ReaderWriterLockSlim();
            this._Registry = new Hashtable();
            this.Name = name;
            this._Parent = parent;
            this._FallBack = fallBack;
            //this._ContainsInstances = string.Equals(ParentNameRoot, this.Name, StringComparison.Ordinal);
            this._ContainsInstances = containsInstances;
        }

        public IDependencyInjectionConfigurable GetParent(string name)
        {
            if (name == null) { name = ""; }
            for (Funcstructor that = this; that != null; that = that._Parent)
            {
                if (that.Name.Equals(name))
                {
                    return that;
                }
            }
            return null;
        }

        private bool registerAny(CacheKey cacheKey, object funcstructor, bool overwrite = false)
        {
            _ReaderWriterLock.EnterWriteLock();
            try
            {
                if (overwrite)
                {
                    _Registry[cacheKey] = new ItemState(cacheKey, funcstructor);
                    return true;
                }
                else
                {
                    if (_Registry[cacheKey] == null)
                    {
                        _Registry[cacheKey] = new ItemState(cacheKey, funcstructor);
                        return true;
                    }
                }
            }
            finally
            {
                _ReaderWriterLock.ExitWriteLock();
            }
            return false;
        }
        private ItemState GetItemState(CacheKey cacheKey)
        {
            ItemState itemState = null;
            for (Funcstructor that = this; that != null; that = that._Parent ?? that._FallBack)
            {
                that._ReaderWriterLock.EnterReadLock();
                try
                {
                    itemState = (ItemState)that._Registry[cacheKey];
                }
                finally
                {
                    that._ReaderWriterLock.ExitReadLock();
                }
                if (itemState != null)
                {
                    return itemState;
                }
            }
            return itemState;
        }

        public bool Register(Type returnType, object key, Type[] parameterTypes, object funcstructor, bool overwrite = false)
        {
            var l = new object[parameterTypes.Length + 2];
            l[0] = key;
            l[1] = returnType;
            for (int idx = 0; idx < parameterTypes.Length; idx++)
            {
                l[2 + idx] = parameterTypes[idx];
            }
            CacheKey cacheKey = new CacheKey(l);
            return this.registerAny(cacheKey, funcstructor, overwrite);
        }
        // 0
        public bool Register<T>(object key, Func<T> funcstructor, bool overwrite = false)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T));
            return this.registerAny(cacheKey, funcstructor, overwrite);
        }
        public T Resolve<T>(object key)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T));
            ItemState itemState = this.GetItemState(cacheKey);
            if ((object)itemState != null)
            {
                var func = (Func<T>)(itemState.Funcstructor);
                return func();
            }
            return default(T);
        }

        // 1
        public void Register<P1, T>(object key, Func<P1, T> funcstructor, bool overwrite = false)
        {
            _ReaderWriterLock.EnterWriteLock();
            try
            {
                CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
                if (overwrite)
                {
                    _Registry[cacheKey] = funcstructor;
                }
                else
                {
                    if (_Registry[cacheKey] == null)
                    {
                        _Registry[cacheKey] = funcstructor;
                    }
                }
            }
            finally
            {
                _ReaderWriterLock.ExitWriteLock();
            }
        }
        public T Resolve<P1, T>(object key, P1 p1)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
            for (Funcstructor that = this; that != null; that = that._Parent ?? that._FallBack)
            {
                Func<P1, T> funcstructor;
                that._ReaderWriterLock.EnterReadLock();
                try
                {
                    funcstructor = (Func<P1, T>)that._Registry[cacheKey];
                }
                finally
                {
                    that._ReaderWriterLock.ExitReadLock();
                }
                if (funcstructor != null)
                {
                    return funcstructor(p1);
                }
            }
            return default(T);
        }
        // 2
        public void Register<P1, P2, T>(object key, Func<P1, P2, T> funcstructor, bool overwrite = false)
        {
            _ReaderWriterLock.EnterWriteLock();
            try
            {
                CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
                if (overwrite)
                {
                    _Registry[cacheKey] = funcstructor;
                }
                else
                {
                    if (_Registry[cacheKey] == null)
                    {
                        _Registry[cacheKey] = funcstructor;
                    }
                }
            }
            finally
            {
                _ReaderWriterLock.ExitWriteLock();
            }
        }
        public T Resolve<P1, P2, T>(object key, P1 p1, P2 p2)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
            for (Funcstructor that = this; that != null; that = that._Parent ?? that._FallBack)
            {
                Func<P1, P2, T> funcstructor;
                that._ReaderWriterLock.EnterReadLock();
                try
                {
                    funcstructor = (Func<P1, P2, T>)that._Registry[cacheKey];
                }
                finally
                {
                    that._ReaderWriterLock.ExitReadLock();
                }
                if (funcstructor != null)
                {
                    return funcstructor(p1, p2);
                }
            }
            return default(T);
        }

        // 3
        public void Register<P1, P2, P3, T>(object key, Func<P1, P2, P3, T> funcstructor, bool overwrite = false)
        {
            _ReaderWriterLock.EnterWriteLock();
            try
            {
                CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
                if (overwrite)
                {
                    _Registry[cacheKey] = funcstructor;
                }
                else
                {
                    if (_Registry[cacheKey] == null)
                    {
                        _Registry[cacheKey] = funcstructor;
                    }
                }
            }
            finally
            {
                _ReaderWriterLock.ExitWriteLock();
            }
        }
        public T Resolve<P1, P2, P3, T>(object key, P1 p1, P2 p2, P3 p3)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
            for (Funcstructor that = this; that != null; that = that._Parent ?? that._FallBack)
            {
                Func<P1, P2, P3, T> funcstructor;
                that._ReaderWriterLock.EnterReadLock();
                try
                {
                    funcstructor = (Func<P1, P2, P3, T>)that._Registry[cacheKey];
                }
                finally
                {
                    that._ReaderWriterLock.ExitReadLock();
                }
                if (funcstructor != null)
                {
                    return funcstructor(p1, p2, p3);
                }
            }
            return default(T);
        }

        //
        public void RegisterLateLoad<T>(object key, string fqnType)
        {
            var that = this.GetParent(ParentNameConfiguration) ?? this;
            that.Register<T>(key, (delegate ()
            {
                that.Register<T>(key, null, true);
                var type = System.Type.GetType(fqnType, true, false);
                that._FallBack.Register<T>(key, () => (T)System.Activator.CreateInstance(type), false);
                return this.Get<T>(key);
            }), false);
        }
        public void RegisterLateLoad<P1, T>(object key, string fqnType)
        {
            var that = this.GetParent(ParentNameConfiguration) ?? this;
            that.Register<P1, T>(key, (delegate (P1 p1)
            {
                that.Register<P1, T>(key, null, true);
                var type = System.Type.GetType(fqnType, true, false);
                that._FallBack.Register<P1, T>(key, (Func<P1, T>)delegate (P1 _p1)
                {
                    return (T)System.Activator.CreateInstance(type, _p1);
                }, false);
                return this.Get<P1, T>(key, p1);
            }), false);
        }
        public void RegisterLateLoad<P1, P2, T>(object key, string fqnType)
        {
            var that = this.GetParent(ParentNameConfiguration) ?? this;
            that.Register<P1, P2, T>(key, (delegate (P1 p1, P2 p2)
            {
                that.Register<P1, P2, T>(key, null, true);
                var type = System.Type.GetType(fqnType, true, false);
                that._FallBack.Register<P1, P2, T>(key, delegate (P1 _p1, P2 _p2)
                {
                    return (T)System.Activator.CreateInstance(type, _p1, _p2);
                }, false);
                return this.Get<P1, P2, T>(key, p1, p2);
            }), false);
        }
        public void RegisterLateLoad<P1, P2, P3, T>(object key, string fqnType)
        {
            var that = this.GetParent(ParentNameConfiguration) ?? this;
            that.Register<P1, P2, P3, T>(key, (delegate (P1 p1, P2 p2, P3 p3)
            {
                that.Register<P1, P2, P3, T>(key, null, true);
                var type = System.Type.GetType(fqnType, true, false);
                that._FallBack.Register<P1, P2, P3, T>(key, delegate (P1 _p1, P2 _p2, P3 _p3)
                {
                    return (T)System.Activator.CreateInstance(type, _p1, _p2, _p3);
                }, false);
                return this.Get<P1, P2, P3, T>(key, p1, p2, p3);
            }), false);
        }

        internal class ItemState
        {
            internal CacheKey CacheKey;
            internal object Funcstructor;

            public ItemState(CacheKey cacheKey, object funcstructor)
            {
                this.CacheKey = cacheKey;
                this.Funcstructor = funcstructor;
            }
        }
    }
}
