using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Bouchon.API.Controllers
{
    [RoutePrefix("api")]
    public class TestController : ApiController
    {
        [Authorize(Roles="Foo")]
        [Route("test")]
        public IHttpActionResult Get()
        {
            return Ok((User as ClaimsPrincipal).Claims);
        }
    }
}
