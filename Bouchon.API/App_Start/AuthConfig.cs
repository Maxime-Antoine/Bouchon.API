using Bouchon.API.Authentication;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Configuration;

namespace Bouchon.API
{
    public class AuthConfig
    {
        public static void Config(IAppBuilder app)
        {
            ////setup identity server
            //app.Map("/identity", idsrvApp =>
            //    {
            //        idsrvApp.UseIdentityServer(new IdentityServerOptions
            //            {
            //                SigningCertificate = LoadCertificate(),
            //                Factory = InMemoryFactory.Create(
            //                    users: Users.Get(),
            //                    clients: Clients.Get(),
            //                    scopes: Scopes.Get()
            //                )
            //            });
            //    });

            ////consume tokens emitted by identity server
            //app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            //    {
            //        Authority = ConfigurationManager.AppSettings["appRootUrl"] + "identity",
            //        RequiredScopes = new string[] { "api" },
            //        AuthenticationMode = AuthenticationMode.Active
            //    });

            _ConfigureJwtTokenGeneration(app);
            _ConfigureJwtTokenConsumption(app);
        }

        //
        private static void _ConfigureJwtTokenGeneration(IAppBuilder app)
        {
            //JWT token generation
            var OAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = false,
                TokenEndpointPath = new Microsoft.Owin.PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(2),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat(ConfigurationManager.AppSettings["appRootUrl"])
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        private static void _ConfigureJwtTokenConsumption(IAppBuilder app)
        {
            //JWT token consumption
            var issuer = ConfigurationManager.AppSettings["appRootUrl"];
            var audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }

        //private static X509Certificate2 LoadCertificate()
        //{
        //    var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
        //    store.Open(OpenFlags.ReadOnly);

        //    foreach (var cert in store.Certificates)
        //        if (cert.FriendlyName == "Development Certificate")
        //            return cert;

        //    throw new Exception("Dev SSL certificate not found !");
        //}
    }
}