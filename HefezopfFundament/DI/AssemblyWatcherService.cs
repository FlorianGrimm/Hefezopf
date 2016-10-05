using Hefezopf.Contracts.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.DI
{
    // AssemblyWatcherService
    public class AssemblyWatcherService : IAssemblyWatcherService
    {
        private bool _IsWired;
        private IDependencyInjectionConfigurable[] _FuncstructorConfigurables;
        private bool _CallFuncstructorConfigurations;
        private List<IFuncstructorConfiguration> _FuncstructorConfigurations;

        public AssemblyWatcherService()
        {
            this._FuncstructorConfigurables = new IDependencyInjectionConfigurable[0];
            this._FuncstructorConfigurations = new List<IFuncstructorConfiguration>();
        }

        public void WireTo(IDependencyInjectionConfigurable funcstructor)
        {
            if (funcstructor == null) { throw new ArgumentNullException(nameof(funcstructor)); }
            lock (this)
            {
                var l = this._FuncstructorConfigurables.ToList();
                l.Add(funcstructor);
                this._FuncstructorConfigurables = l.ToArray();
            }
            this.Wire();
        }
        //
        public void Wire()
        {
            //if (!_ContainsInstances) { throw new InvalidOperationException("can only be called on a Root"); }
            if (this._IsWired) { return; }
            this._IsWired = true;
            System.AppDomain.CurrentDomain.AssemblyLoad += this.CurrentDomain_AssemblyLoad;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies().ToArray())
            {
                this.ScanAssembly(assembly);
            }
            this.CallFuncstructorConfigurations();
        }
        public void Unwire()
        {
            if (!this._IsWired) { return; }
            this._IsWired = false;
            System.AppDomain.CurrentDomain.AssemblyLoad -= this.CurrentDomain_AssemblyLoad;
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var assembly = args.LoadedAssembly;
            this.ScanAssembly(assembly);
            this.CallFuncstructorConfigurations();
        }

        private void ScanAssembly(System.Reflection.Assembly assembly)
        {
            var name = assembly.GetName();
            var assembly_Name = name.Name;
            if (assembly_Name.StartsWith("System.")) { return; }
            if (assembly_Name.StartsWith("Microsoft.")) { return; }
            var publicKeyToken = name.GetPublicKeyToken();
            if (publicKeyToken != null)
            {
                if ((publicKeyToken[0] == 0xb7)
                    && (publicKeyToken[1] == 0x7a)
                    && (publicKeyToken[2] == 0x5c)
                    && (publicKeyToken[3] == 0x56)
                    && (publicKeyToken[4] == 0x19)
                    && (publicKeyToken[5] == 0x34)
                    && (publicKeyToken[6] == 0xe0)
                    && (publicKeyToken[7] == 0x89))
                {
                    return;
                }
                if ((publicKeyToken[0] == 0xb0)
                    && (publicKeyToken[1] == 0x3f)
                    && (publicKeyToken[2] == 0x5f)
                    && (publicKeyToken[3] == 0x7f)
                    && (publicKeyToken[4] == 0x11)
                    && (publicKeyToken[5] == 0xd5)
                    && (publicKeyToken[6] == 0x0a)
                    && (publicKeyToken[7] == 0x3a))
                {
                    return;
                }
                //
            }
            var hefezopfContractsAssemblyName = typeof(FuncstructorConfigurationAttribute).Assembly.GetName().Name;
            bool found = false;
            foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
            {
                if (string.Equals(referencedAssemblyName.Name, hefezopfContractsAssemblyName, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                }
            }
            if (!found) { return; }
            //
            //var assembly_FullName = assembly.FullName;
            {
                var attrs = assembly.GetCustomAttributes<FuncstructorConfigurationAttribute>();
                foreach (var attr in attrs)
                {
                    var registerType = attr.RegisterType;

                    var constructor = registerType.GetConstructor(Type.EmptyTypes);
                    var instance = (IFuncstructorConfiguration)constructor.Invoke(new object[0]);
                    this._FuncstructorConfigurations.Add(instance);
                }
            }
            {
                var attrs = assembly.GetCustomAttributes<FuncstructorFactoryAttribute>();
                foreach (var attr in attrs)
                {
                    var factoryType = attr.FactoryType;
                    //
                    var methods = factoryType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    foreach (var method in methods)
                    {
                        var methodAttrs = method.GetCustomAttributes<FuncstructorRegisterAttribute>(false);
                        foreach (var methodAttr in methodAttrs)
                        {
                            //method.Invoke(null, parameters)
                            this._FuncstructorConfigurations.Add(new MethodFuncstructorConfiguration(method, methodAttr.Key));
                        }
                    }
                }
            }
        }

        private void CallFuncstructorConfigurations()
        {
            if (this._CallFuncstructorConfigurations) { return; }
            //
            this._CallFuncstructorConfigurations = true;
            try
            {
                while (this._FuncstructorConfigurations.Count > 0)
                {
                    var funcstructorConfigurations = System.Threading.Interlocked.Exchange(
                        ref this._FuncstructorConfigurations,
                        new List<IFuncstructorConfiguration>());
                    System.Threading.Interlocked.MemoryBarrier();
                    if (funcstructorConfigurations.Count > 0)
                    {
                        var funcstructorConfigurables = this._FuncstructorConfigurables;
                        var funcstructorConfigurablesLength = funcstructorConfigurables.Length;
                        foreach (var funcstructorConfiguration in funcstructorConfigurations)
                        {
                            for (int idxFunc = 0; idxFunc < funcstructorConfigurablesLength; idxFunc++)
                            {
                                IDependencyInjectionConfigurable funcstructorConfigurable = funcstructorConfigurables[idxFunc];
                                funcstructorConfiguration.Register(funcstructorConfigurable);
                            }
                        }
                    }
                }
            }
            finally
            {
                this._CallFuncstructorConfigurations = false;
            }
        }
        internal class MethodFuncstructorConfiguration : IFuncstructorConfiguration
        {
            private object _Key;
            private MethodInfo _MethodInfo;

            public MethodFuncstructorConfiguration(MethodInfo method, object key)
            {
                this._MethodInfo = method;
                this._Key = key;
            }

            public void Register(IDependencyInjectionConfigurable funcstructor)
            {
                var returnType = this._MethodInfo.ReturnType;
                var methodParameters = this._MethodInfo.GetParameters();
                var parameterTypes = new Type[methodParameters.Length];
                var genericParameterTypes = new Type[methodParameters.Length + 1];
                for (int idx = 0; idx < methodParameters.Length; idx++)
                {
                    var methodParameter = methodParameters[idx];
                    var parameterType = methodParameter.ParameterType;
                    parameterTypes[idx] = parameterType;
                    genericParameterTypes[idx] = parameterType;
                }
                genericParameterTypes[methodParameters.Length] = returnType;
                //
                Type typeFunc;
                switch (methodParameters.Length)
                {
                    case 0:
                        typeFunc = typeof(Func<>);
                        break;
                    case 1:
                        typeFunc = typeof(Func<,>);
                        break;
                    case 2:
                        typeFunc = typeof(Func<,,>);
                        break;
                    case 3:
                        typeFunc = typeof(Func<,,,>);
                        break;
                    case 4:
                        typeFunc = typeof(Func<,,,,>);
                        break;
                    case 5:
                        typeFunc = typeof(Func<,,,,,>);
                        break;
                    case 6:
                        typeFunc = typeof(Func<,,,,,,>);
                        break;
                    default:
                        throw new NotSupportedException();
                }
                var typeFuncT = typeFunc.MakeGenericType(genericParameterTypes);
                var delegateT = Delegate.CreateDelegate(typeFuncT, this._MethodInfo);
                funcstructor.Register(returnType, this._Key, parameterTypes, delegateT, false);
            }
        }
    }
}
