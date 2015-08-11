using Microsoft.AspNet.Identity;
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Bouchon.API.Authentication
{
    public class AuthMailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            dynamic email = new Email("AuthEmail");
            email.To = message.Destination;
            email.Body = message.Body;
            email.Subject = message.Subject;

            email.Send();

            return Task.FromResult(0);
        }
    }
}