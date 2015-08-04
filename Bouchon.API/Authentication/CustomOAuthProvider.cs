using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bouchon.API.Authentication
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var scope = context.OwinContext.Get<Autofac.Core.Lifetime.LifetimeScope>("autofac:OwinLifetimeScope");
            var usrMgr = scope.GetService(typeof(ApplicationUserManager)) as ApplicationUserManager;

            ApplicationUser user = await usrMgr.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The username or password is incorrect.");
                return;
            }

            if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "User did not confirm email.");
            }

            ClaimsIdentity oauthIdentity = await user.GenerateUserIdentityAsync(usrMgr, "JWT");

            var ticket = new AuthenticationTicket(oauthIdentity, null);

            context.Validated(ticket);
        }
    }
}