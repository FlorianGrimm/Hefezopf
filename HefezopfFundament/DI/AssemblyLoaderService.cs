using Hefezopf.Contracts.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.DI
{
    public class AssemblyLoaderService : IAssemblyLoaderService
    {
        private bool _IsWired;
        private IDependencyInjectionConfigurable[] _Funcstructors;
        private readonly List<IFuncstructorConfiguration> _FuncstructorConfigurations;
        public AssemblyLoaderService()
        {
            this._Funcstructors = new IDependencyInjectionConfigurable[0];
            this._FuncstructorConfigurations = new List<IFuncstructorConfiguration>();
        }

        public void WireTo(IDependencyInjectionConfigurable funcstructor)
        {
            if (funcstructor == null) { throw new ArgumentNullException(nameof(funcstructor)); }
            lock (this)
            {
                var l = this._Funcstructors.ToList();
                l.Add(funcstructor);
                this._Funcstructors = l.ToArray();
            }
            this.Wire();
        }
        //
        public void Wire()
        {
            //if (!_ContainsInstances) { throw new InvalidOperationException("can only be called on a Root"); }
            if (_IsWired) { return; }
            _IsWired = true;
            System.AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies().ToArray())
            {
                ScanAssembly(assembly);
            }
        }
        public void Unwire()
        {
            if (!_IsWired) { return; }
            _IsWired = false;
            System.AppDomain.CurrentDomain.AssemblyLoad -= CurrentDomain_AssemblyLoad;
        }

        void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var assembly = args.LoadedAssembly;
            this.ScanAssembly(assembly);
            this.CallFuncstructorConfigurations();
        }

        private void ScanAssembly(System.Reflection.Assembly assembly)
        {
            var attrs = assembly.GetCustomAttributes(typeof(FuncstructorRegisterAttribute), false);
            for (int idx = 0; idx < attrs.Length; idx++)
            {
                var attr = (FuncstructorRegisterAttribute)attrs[idx];
                var instance = (IFuncstructorConfiguration)System.Activator.CreateInstance(attr.RegisterType);
                lock (this._FuncstructorConfigurations)
                {
                    this._FuncstructorConfigurations.Add(instance);
                }
            }
        }
        private void CallFuncstructorConfigurations()
        {
            var funcstructorConfigurations = new List<IFuncstructorConfiguration>();
            lock (this._FuncstructorConfigurations)
            {
                funcstructorConfigurations.AddRange(this._FuncstructorConfigurations);
                this._FuncstructorConfigurations.Clear();
            }
            var funcstructors = this._Funcstructors;
            foreach (var funcstructorConfiguration in funcstructorConfigurations)
            {
                for (int idxFunc = 0; idxFunc < funcstructors.Length; idxFunc++)
                {
                    funcstructorConfiguration.Register(funcstructors[idxFunc]);
                }
            }
        }
    }
}
