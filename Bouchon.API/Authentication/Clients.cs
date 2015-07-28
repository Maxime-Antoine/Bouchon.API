using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinktecture.IdentityServer.Core.Models;

namespace Bouchon.API.Authentication
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "Bouchon Website",
                    ClientId = "bouchon.website",
                    ClientSecrets = new List<ClientSecret>
                    {   
                        new ClientSecret("1a2a4b72-dda2-4dc8-94df-77f5ef7a84b2".Sha256())
                    },
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Jwt,
                    Flow = Flows.ResourceOwner
                }
            };
        }
    }
}