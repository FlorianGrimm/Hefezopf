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
#pragma warning disable SA1313 // Parameter names must begin with lower-case letter

    public class Funcstructor
        : IDependencyInjection
        , IDependencyInjectionConfigurable
    {
        public const string ParentNameRoot = "Root";
        public const string ParentNameLateInstance = "LateInstance";
        public const string ParentNameLateLoad = "LateLoad";

        public readonly string Name;
        private readonly bool _ContainsInstances;
        private ReaderWriterLockSlim _ReaderWriterLock;
        private Hashtable _RegistryItemState;
        private Funcstructor _Parent;

        public Funcstructor(string name, Funcstructor parent)
        {
            //if (ParentNameFallBack.Equals(name)) { throw new ArgumentException("Funcstructor-FallBack is not allowed", "name"); }
            this._ReaderWriterLock = new ReaderWriterLockSlim();
            this._RegistryItemState = new Hashtable();
            this.Name = name ?? string.Empty;
            this._Parent = parent;
            this._ContainsInstances = true;
            //this._FallBack = new Funcstructor(ParentNameFallBack, null);
        }

        private Funcstructor(string name, Funcstructor parent, bool containsInstances)
        {
            this._ReaderWriterLock = new ReaderWriterLockSlim();
            this._RegistryItemState = new Hashtable();
            this.Name = name;
            this._Parent = parent;
            //this._ContainsInstances = string.Equals(ParentNameRoot, this.Name, StringComparison.Ordinal);
            this._ContainsInstances = containsInstances;
        }

        public static Funcstructor CreateDefault(IAssemblyWatcherService assemblyWatcherService)
        {
            var instanceConfiguration = new Funcstructor(ParentNameLateLoad, null,  false);
            var instanceLateInstance = new Funcstructor(ParentNameLateInstance, instanceConfiguration, false);
            var instanceRoot = new Funcstructor(ParentNameRoot, instanceLateInstance, true);
            if ((object)assemblyWatcherService == null)
            {
                assemblyWatcherService.WireTo(instanceRoot);
            }
            return instanceRoot;
        }

        public IDependencyInjectionConfigurable GetParent(string name)
        {
            if (name == null) { name = string.Empty; }
            for (Funcstructor that = this; that != null; that = that._Parent)
            {
                if (that.Name.Equals(name))
                {
                    return that;
                }
            }
            return null;
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
        public bool Register<P1, T>(object key, Func<P1, T> funcstructor, bool overwrite = false)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
            return this.registerAny(cacheKey, funcstructor, overwrite);
        }
        public T Resolve<P1, T>(object key, P1 p1)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
            ItemState itemState = this.GetItemState(cacheKey);
            if ((object)itemState != null)
            {
                var func = (Func<P1, T>)(itemState.Funcstructor);
                return func(p1);
            }
            return default(T);
        }
        // 2
        public bool Register<P1, P2, T>(object key, Func<P1, P2, T> funcstructor, bool overwrite = false)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
            return this.registerAny(cacheKey, funcstructor, overwrite);
        }
        public T Resolve<P1, P2, T>(object key, P1 p1, P2 p2)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
            ItemState itemState = this.GetItemState(cacheKey);
            if ((object)itemState != null)
            {
                var func = (Func<P1, P2, T>)(itemState.Funcstructor);
                return func(p1, p2);
            }
            return default(T);
        }

        // 3
        public bool Register<P1, P2, P3, T>(object key, Func<P1, P2, P3, T> funcstructor, bool overwrite = false)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
            return this.registerAny(cacheKey, funcstructor, overwrite);
        }
        public T Resolve<P1, P2, P3, T>(object key, P1 p1, P2 p2, P3 p3)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T), typeof(P1));
            ItemState itemState = this.GetItemState(cacheKey);
            if ((object)itemState != null)
            {
                var func = (Func<P1, P2, P3, T>)(itemState.Funcstructor);
                return func(p1, p2, p3);
            }
            return default(T);
        }

        //
        public void RegisterLateLoad<T>(object key, string fqnType)
        {
            var lateLoad = this.GetParent(ParentNameLateLoad) ?? this;
            var itemState = new ItemStateLateLoad(key, fqnType);
            Func<T> funcLateLoad = (delegate ()
                {
                    var lateInstance = this.GetParent(ParentNameLateInstance) ?? this;
                    Func<T> funcLateInstance = (delegate ()
                    {
                        return (T)System.Activator.CreateInstance(itemState.GetClassType());
                    });
                    lateInstance.Register<T>(key, funcLateInstance, false);
                    return this.Resolve<T>(itemState.Key);
                });
            itemState.Funcstructor = funcLateLoad;
            this.Register(typeof(T), key, null, itemState, false);
        }
        public void RegisterLateLoad<P1, T>(object key, string fqnType)
        {
            var lateLoad = this.GetParent(ParentNameLateLoad) ?? this;
            var itemState = new ItemStateLateLoad(key, fqnType);
            Func<P1, T> funcLateLoad = (delegate (P1 l1)
            {
                var lateInstance = this.GetParent(ParentNameLateInstance) ?? this;
                Func<P1, T> funcLateInstance = (delegate (P1 i1)
                {
                    return (T)System.Activator.CreateInstance(itemState.GetClassType(), i1);
                });
                lateInstance.Register<P1, T>(key, funcLateInstance, false);
                return this.Resolve<P1, T>(itemState.Key, l1);
            });
            itemState.Funcstructor = funcLateLoad;
            this.Register(typeof(T), key, new Type[] { typeof(P1) }, itemState, false);
        }
        public void RegisterLateLoad<P1, P2, T>(object key, string fqnType)
        {
            var lateLoad = this.GetParent(ParentNameLateLoad) ?? this;
            var itemState = new ItemStateLateLoad(key, fqnType);
            Func<P1, P2, T> funcLateLoad = (delegate (P1 l1, P2 l2)
            {
                var lateInstance = this.GetParent(ParentNameLateInstance) ?? this;
                Func<P1, P2, T> funcLateInstance = (delegate (P1 i1, P2 i2)
                {
                    return (T)System.Activator.CreateInstance(itemState.GetClassType(), i1, i2);
                });
                lateInstance.Register<P1, P2, T>(key, funcLateInstance, false);
                return this.Resolve<P1, P2, T>(itemState.Key, l1, l2);
            });
            itemState.Funcstructor = funcLateLoad;
            this.Register(typeof(T), key, new Type[] { typeof(P1), typeof(P2) }, itemState, false);
        }
        public void RegisterLateLoad<P1, P2, P3, T>(object key, string fqnType)
        {
            var lateLoad = this.GetParent(ParentNameLateLoad) ?? this;
            var itemState = new ItemStateLateLoad(key, fqnType);
            Func<P1, P2, P3, T> funcLateLoad = (delegate (P1 l1, P2 l2, P3 l3)
            {
                var lateInstance = this.GetParent(ParentNameLateInstance) ?? this;
                Func<P1, P2, P3, T> funcLateInstance = (delegate (P1 i1, P2 i2, P3 i3)
                {
                    return (T)System.Activator.CreateInstance(itemState.GetClassType(), i1, i2, i3);
                });
                lateInstance.Register<P1, P2, P3, T>(key, funcLateInstance, false);
                return this.Resolve<P1, P2, P3, T>(itemState.Key, l1, l2, l3);
            });
            itemState.Funcstructor = funcLateLoad;
            this.Register(typeof(T), key, new Type[] { typeof(P1), typeof(P2), typeof(P3) }, itemState, false);
        }

        private bool registerAny(CacheKey cacheKey, object funcstructor, bool overwrite = false)
        {
            this._ReaderWriterLock.EnterWriteLock();
            try
            {
                if (overwrite)
                {
                    var itemState = funcstructor as ItemState;
                    if (itemState != null)
                    {
                        itemState.CacheKey = cacheKey;
                    }
                    else
                    {
                        itemState = new ItemState(cacheKey, funcstructor);
                    }
                    this._RegistryItemState[cacheKey] = itemState;
                    return true;
                }
                else
                {
                    if (this._RegistryItemState[cacheKey] == null)
                    {
                        var itemState = funcstructor as ItemState;
                        if (itemState != null)
                        {
                            itemState.CacheKey = cacheKey;
                        }
                        else
                        {
                            itemState = new ItemState(cacheKey, funcstructor);
                        }
                        this._RegistryItemState[cacheKey] = itemState;
                        return true;
                    }
                }
            }
            finally
            {
                this._ReaderWriterLock.ExitWriteLock();
            }
            return false;
        }
        private ItemState GetItemState(CacheKey cacheKey)
        {
            ItemState itemState = null;
            for (Funcstructor that = this; that != null; that = that._Parent)
            {
                that._ReaderWriterLock.EnterReadLock();
                try
                {
                    itemState = (ItemState)that._RegistryItemState[cacheKey];
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

        internal class ItemStateLateLoad : ItemState
        {
            internal object Key;
            internal string ClassName;
            internal Type Type;

            public ItemStateLateLoad(object key, string className)
                : base(null, null)
            {
                this.Key = key;
                this.ClassName = className;
                this.Funcstructor = null;
            }
            internal Type GetClassType()
            {
                if (this.Type == null)
                {
                    //base.Prepare();
                    this.Type = System.Type.GetType(this.ClassName, true);
                }
                return this.Type;
            }
        }
    }
}
