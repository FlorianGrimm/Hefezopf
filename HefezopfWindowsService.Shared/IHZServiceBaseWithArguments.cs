using System;
using System.Collections.Generic;
using System.Text;

namespace HefezopfWindowsService.Shared
{
    public interface IHZServiceBaseWithArguments {
        IDisposable Debug(IEnumerable<string> args);
        void SetArguments(IEnumerable<string> args);
    }
    public interface IHZService : IDisposable {
        void Start(object args);

        void Stop();
    }
}
