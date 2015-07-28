using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(Bouchon.API.Startup))]

namespace Bouchon.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);
            app.UseCors(CorsOptions.AllowAll); //Allow CORS for API

            IocConfig.Config(app, config);

            AuthConfig.Config(app);

            app.UseWebApi(config);
        }
    }
}