namespace Hefezopf.Contracts.DI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Brimborium.Funcstructors;

    /// <summary>
    /// The dependency injection service.
    /// </summary>
    public class DIService : IDIService
    {
        /// <summary>
        /// the global static instance - for testing.
        /// </summary>
        public static IDIService DIServiceInstance;

        /// <summary>
        /// the factory for the instance.
        /// </summary>
        public static Func<IDIService> DIServiceFactory;

        /// <summary>
        /// Gets the global instance of the funcstructor.
        /// </summary>
        public static IFuncstructor GlobalFuncstructor
        {
            get
            {
                var diService = DIServiceInstance;
                if ((object)diService != null)
                {
                    return diService.GetGlobalFuncstructor();
                }
                {
                    diService = CreateDIServiceInstance();
                    var diServiceUsed = System.Threading.Interlocked.CompareExchange(ref DIServiceInstance, diService, null) ?? diService;
                    return diServiceUsed.GetGlobalFuncstructor();
                }
            }
        }

        /// <summary>
        /// Create a new instance of IDIService.
        /// </summary>
        /// <returns>a new instance of IDIService.</returns>
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

        /// <summary>
        /// Gets the name of the default implementation
        /// </summary>
        /// <returns>The assembly qualified name.</returns>
        public static string GetDefaultImplementationTypeName()
        {
            return $"Hefezopf.Fundament.DI.DIServiceImplementation , Hefezopf.Fundament, Version={Hefezopf.HZVersion.AssemblyVersion}, Culture=neutral, PublicKeyToken=2d8ec664ab7d64f1";
        }

        /// <summary>
        /// Get an old or new instance of funcstructor.
        /// </summary>
        /// <returns>a funcstructor.</returns>
        public virtual IFuncstructor GetGlobalFuncstructor()
        {
            throw new NotImplementedException();
        }
    }
}
