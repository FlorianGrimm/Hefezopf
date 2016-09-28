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
            if ((object)assemblyLoaderService == null) {
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

        public Funcstructor GetParent(string name)
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

        // 0
        public bool Register<T>(object key, Func<T> funcstructor, bool overwrite = false)
        {
            _ReaderWriterLock.EnterWriteLock();
            try
            {
                CacheKey cacheKey = new CacheKey(key, typeof(T));
                if (overwrite)
                {
                    _Registry[cacheKey] = funcstructor;
                    return true;
                }
                else
                {
                    if (_Registry[cacheKey] == null)
                    {
                        _Registry[cacheKey] = funcstructor;
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
        public bool Register<T>(string parentName, object key, Func<T> funcstructor, bool overwrite = false)
        {
            return this.GetParent(parentName).Register<T>(key, funcstructor, overwrite);
        }
        public T Get<T>(object key)
        {
            CacheKey cacheKey = new CacheKey(key, typeof(T));
            for (Funcstructor that = this; that != null; that = that._Parent ?? that._FallBack)
            {
                Func<T> funcstructor;
                that._ReaderWriterLock.EnterReadLock();
                try
                {
                    funcstructor = (Func<T>)that._Registry[cacheKey];
                }
                finally
                {
                    that._ReaderWriterLock.ExitReadLock();
                }
                if (funcstructor != null)
                {
                    return funcstructor();
                }
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
        public void Register<P1, T>(string parentName, object key, Func<P1, T> funcstructor, bool overwrite = false)
        {
            this.GetParent(parentName).Register<P1, T>(key, funcstructor, overwrite);
        }
        public T Get<P1, T>(object key, P1 p1)
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
        public void Register<P1, P2, T>(string parentName, object key, Func<P1, P2, T> funcstructor, bool overwrite = false)
        {
            this.GetParent(parentName).Register<P1, P2, T>(key, funcstructor, overwrite);
        }
        public T Get<P1, P2, T>(object key, P1 p1, P2 p2)
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
        public void Register<P1, P2, P3, T>(string parentName, object key, Func<P1, P2, P3, T> funcstructor, bool overwrite = false)
        {
            this.GetParent(parentName).Register<P1, P2, P3, T>(key, funcstructor, overwrite);
        }
        public T Get<P1, P2, P3, T>(object key, P1 p1, P2 p2, P3 p3)
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
#if weichei
        //
        private bool isWired;
        public void Wire()
        {
            if (!_ContainsInstances) { throw new InvalidOperationException("can only be called on a Root"); }
            if (isWired) { return; }
            isWired = true;
            System.AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies().ToArray())
            {
                ScanAssembly(assembly);
            }
        }
        public void Unwire()
        {
            if (!isWired) { return; }
            isWired = false;
            System.AppDomain.CurrentDomain.AssemblyLoad -= CurrentDomain_AssemblyLoad;
        }

        void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var assembly = args.LoadedAssembly;
            ScanAssembly(assembly);
        }

        private void ScanAssembly(System.Reflection.Assembly assembly)
        {
            var attrs = assembly.GetCustomAttributes(typeof(FuncstructorRegisterAttribute), false);
            for (int idx = 0; idx < attrs.Length; idx++)
            {
                var attr = (FuncstructorRegisterAttribute)attrs[idx];
                var instance = (IFuncstructorConfiguration)System.Activator.CreateInstance(attr.RegisterType);
                instance.Register(this);
            }
        }
#endif
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
    }
}
