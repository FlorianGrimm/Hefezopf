using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.DI
{
    public interface IDependencyInjection
    {
        // 0
        T Resolve<T>(object key);

        // 1
        T Resolve<P1, T>(object key, P1 p1);

        // 2
        T Resolve<P1, P2, T>(object key, P1 p1, P2 p2);

        // 3
        T Resolve<P1, P2, P3, T>(object key, P1 p1, P2 p2, P3 p3);
    }

    public interface IDependencyInjectionConfigurable : IDependencyInjection
    {
        IDependencyInjectionConfigurable GetParent(string name);

        bool Register(Type returnType, object key, Type[] parameterTypes, object funcstructor, bool overwrite = false);

        // 0
        bool Register<T>(object key, Func<T> funcstructor, bool overwrite = false);

        // 1
        bool Register<P1, T>(object key, Func<P1, T> funcstructor, bool overwrite = false);

        // 2
        bool Register<P1, P2, T>(object key, Func<P1, P2, T> funcstructor, bool overwrite = false);

        // 3
        bool Register<P1, P2, P3, T>(object key, Func<P1, P2, P3, T> funcstructor, bool overwrite = false);
    }
}
