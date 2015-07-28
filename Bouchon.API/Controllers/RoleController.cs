using Bouchon.API.Authentication;
using Bouchon.API.BindingModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bouchon.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/role")]
    public class RoleController : ApiController
    {
        private ApplicationUserManager _usrMgr;
        private ApplicationRoleManager _roleMgr;

        public RoleController(ApplicationUserManager usrMgr, ApplicationRoleManager roleMgr)
        {
            _usrMgr = usrMgr;
            _roleMgr = roleMgr;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var roles = await _roleMgr.Roles.ToListAsync();

            return Ok(roles);
        }

        [HttpGet]
        [Route("{id:guid}", Name = "GetRoleById")]
        public async Task<IHttpActionResult> GetRoleById(string id)
        {
            var role = await _roleMgr.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CreateRoleBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = new IdentityRole { Name = model.Name };
            var result = await _roleMgr.CreateAsync(role);

            if (!result.Succeeded)
                return InternalServerError(new Exception("Error creating role"));

            Uri location = new Uri(Url.Link("GetRoleById", new { id = role.Id }));

            return Created(location, role);
        }

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var role = await _roleMgr.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            IdentityResult result = await _roleMgr.DeleteAsync(role);

            if (!result.Succeeded)
                return InternalServerError(new Exception("Error deleting role"));

            return Ok();
        }

        [HttpPost]
        [Route("ManageUsersInRole")]
        public async Task<IHttpActionResult> ManageUsersInRole(UsersInRoleBindingModel model)
        {
            var role = await _roleMgr.FindByIdAsync(model.Id);

            if (role == null)
            {
                ModelState.AddModelError("", "Role not found");
                return BadRequest(ModelState);
            }

            foreach (var user in model.EnrolledUsers)
            {
                var appUser = await _usrMgr.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", String.Format("User: {0} does not exist", user));
                    continue;
                }

                if (!await _usrMgr.IsInRoleAsync(user, role.Name))
                {
                    var result = await _usrMgr.AddToRoleAsync(user, role.Name);

                    if (!result.Succeeded)
                        ModelState.AddModelError("", String.Format("User: {0} could not be added to role", user));
                }
            }

            foreach (var user in model.RemovedUsers)
            {
                var appUser = await _usrMgr.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", String.Format("User: {0} does not exists", user));
                    continue;
                }

                IdentityResult result = await _usrMgr.RemoveFromRoleAsync(user, role.Name);

                if (!result.Succeeded)
                    ModelState.AddModelError("", String.Format("User: {0} could not be removed from role", user));
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok();
        }
    }
}