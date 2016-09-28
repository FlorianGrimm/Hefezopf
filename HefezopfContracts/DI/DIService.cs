using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.DI
{
    public class DIService
    {
        public static IDIService DIServiceInstance;
        public static Func<IDIService> DIServiceFactory;

        public static IDependencyInjection GlobalInstance
        {
            get
            {
                var diService = DIServiceInstance;
                if ((object)diService != null)
                {
                    return diService.GetGlobalInstance();
                }
                {
                    diService = CreateDIServiceInstance();
                    var diServiceUsed = System.Threading.Interlocked.CompareExchange(ref DIServiceInstance, diService, null) ?? diService;
                    return diServiceUsed.GetGlobalInstance();
                }
            }
        }

        public static IDIService CreateDIServiceInstance()
        {
            IDIService result = null;
            if ((object)DIServiceFactory != null)
            {
                result = DIServiceFactory();
            }
            if ((object)result == null)
            {
                string typeName = GetDefaultImplementationTypeName();
                result = (IDIService)(System.Type.GetType(typeName, true).GetConstructor(Type.EmptyTypes).Invoke(new object[0]));
            }
            return result;
        }

        public static string GetDefaultImplementationTypeName()
        {
            return $"Hefezopf.Fundament.DI.DIServiceImplementation , Hefezopf.Fundament, Version={Hefezopf.HZVersion.AssemblyVersion}, Culture=neutral, PublicKeyToken=2d8ec664ab7d64f1";
        }

        public virtual IDependencyInjection GetGlobalInstance()
        {
            throw new NotImplementedException();
        }
    }
}
