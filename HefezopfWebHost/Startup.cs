namespace Hefezopf.WebHost {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.Owin;
    using Owin;
    //using MinimalOwinWebApiSelfHost.Models;
    //using MinimalOwinWebApiSelfHost.OAuthServerProvider;
    //using Microsoft.Owin.Security.OAuth;

    public class Startup {
        private HttpConfiguration _HttpConfiguration;

        public Startup() {
            this._HttpConfiguration = new HttpConfiguration();
        }

        public void Configuration(IAppBuilder app) {
            //this.ConfigureAuth(app);
            this.ConfigureWebApi();
            app.UseWebApi(this._HttpConfiguration);
        }

        /*
        private void ConfigureAuth(IAppBuilder app) {
            var OAuthOptions = new OAuthAuthorizationServerOptions {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),

                // Only do this for demo!!
                AllowInsecureHttp = true
            };
            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
        */

        private void ConfigureWebApi() {
            var config = this._HttpConfiguration;
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });            
        }

        /// <summary>
        /// 
        /// </summary>
        public HttpConfiguration HttpConfiguration => this._HttpConfiguration;
    }
}
