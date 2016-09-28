using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts
{
   public class DIService
    {
        protected static DIService _DIService;
        public static IDependencyInjection GlobalInstance {
            get {
                return _DIService.GetGlobalInstance();
            }
        }
        public virtual IDependencyInjection GetGlobalInstance() {
            return null;
        }
    }
    public interface IDependencyInjection {
    }
}
