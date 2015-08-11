using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Configuration;
using System.Net.Configuration;
using System.Web.Configuration;
using System.Web.Http;

[assembly: OwinStartup(typeof(Bouchon.API.Startup))]

namespace Bouchon.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll); //Allow CORS for API

            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);

            IocConfig.Config(app, config);

            AuthConfig.Config(app);

            app.UseWebApi(config);

            //configure SMTP
            //var webconfig = WebConfigurationManager.OpenWebConfiguration("~") as System.Configuration.Configuration;
            //var settings = webconfig.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;
            //settings.Smtp.Network.Port = Int32.Parse(ConfigurationManager.AppSettings["sg:port"]);
            //settings.Smtp.Network.Host = ConfigurationManager.AppSettings["sg:server"];
            //settings.Smtp.Network.UserName = ConfigurationManager.AppSettings["sg:username"];
            //settings.Smtp.Network.Password = ConfigurationManager.AppSettings["sg:password"];
            //webconfig.Save();
        }
    }
}