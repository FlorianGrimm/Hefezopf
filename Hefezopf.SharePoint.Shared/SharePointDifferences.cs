namespace Hefezopf.SharePoint.Shared {
    using Microsoft.SharePoint.Administration;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class SharePointDifferences {
        public static bool ServerHasRoleForServiceInstance(SPServer server) {
#if SP2013
            return (server.Role == SPServerRole.Application || server.Role == SPServerRole.SingleServer || server.Role == SPServerRole.WebFrontEnd);
#elif SP2016
            return (server.Role == SPServerRole.Application || server.Role == SPServerRole.ApplicationWithSearch || server.Role == SPServerRole.WebFrontEnd || server.Role == SPServerRole.SingleServerFarm);
#else
#error SP2013 SP2016 not defined
#endif

        }
    }
}
