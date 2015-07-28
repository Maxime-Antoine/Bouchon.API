using Bouchon.API.Authentication;
using Microsoft.Owin.Security;
using Owin;
using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Thinktecture.IdentityServer.AccessTokenValidation;
using Thinktecture.IdentityServer.Core.Configuration;

namespace Bouchon.API
{
    public class AuthConfig
    {
        public static void Config(IAppBuilder app)
        {
            //setup identity server
            app.Map("/identity", idsrvApp =>
                {
                    idsrvApp.UseIdentityServer(new IdentityServerOptions
                        {
                            SigningCertificate = LoadCertificate(),
                            Factory = InMemoryFactory.Create(
                                users: Users.Get(),
                                clients: Clients.Get(),
                                scopes: Scopes.Get()
                            )
                        });
                });

            //consume tokens emitted by identity server
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
                {
                    Authority = ConfigurationManager.AppSettings["appRootUrl"] + "identity",
                    RequiredScopes = new string[] { "api" },
                    AuthenticationMode = AuthenticationMode.Active
                });
        }

        private static X509Certificate2 LoadCertificate()
        {
            var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            foreach (var cert in store.Certificates)
                if (cert.FriendlyName == "Development Certificate")
                    return cert;

            throw new Exception("Dev SSL certificate not found !");
        }
    }
}