using Autofac;
using Autofac.Integration.WebApi;
using Bouchon.API.Authentication;
using Bouchon.API.Db;
using Microsoft.AspNet.Identity.EntityFramework;
using Owin;
using Postal;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;

namespace Bouchon.API
{
    public class IocConfig
    {
        public static void Config(IAppBuilder app, HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            //register controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()) //Register ***Services
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces();

            //Register Auth classes
            builder.RegisterType<AppDbContext>().As<DbContext>().InstancePerRequest();
            builder.RegisterType<UserStore<ApplicationUser>>().AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<RoleStore<IdentityRole>>().AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterType<ApplicationRoleManager>().AsSelf().InstancePerRequest();

            //Register generics
            builder.RegisterGeneric(typeof(Repository<>)).AsImplementedInterfaces();

            // Register dependecy of MailerApiController from Postal dll
            builder.RegisterType<EmailService>().As<IEmailService>();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container); //WebAPI

            app.UseAutofacMiddleware(container); //Register before MVC / WebApi
            app.UseAutofacWebApi(config);
        }
    }
}