using Bouchon.API.Authentication;
using Bouchon.API.BindingModels;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bouchon.API.Controllers
{
    //[Authorize]
    [RoutePrefix("api")]
    public class AccountController : ApiController
    {
        private ApplicationUserManager _userMgr;
        private ApplicationRoleManager _roleMgr;

        public AccountController(ApplicationUserManager userMgr, ApplicationRoleManager roleMgr)
        {
            _roleMgr = roleMgr;
            _userMgr = userMgr;
        }

        [HttpGet]
        [Route("user")]
        public async Task<IHttpActionResult> GetUsers()
        {
            var users = await _userMgr.Users.ToListAsync();

            return Ok(users);
        }

        [HttpGet]
        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> GetUserById(string id)
        {
            var user = await _userMgr.FindByIdAsync(id);

            if (user != null)
                return Ok(user);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await _userMgr.FindByNameAsync(username);

            if (user != null)
                return Ok(user);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("user/{email:regex(^\\S+@\\S+\\.\\S+$)}")]
        public async Task<IHttpActionResult> GetUserByEmail(string email)
        {
            var user = await _userMgr.FindByEmailAsync(email);

            if (user != null)
                return Ok(user);
            else
                return NotFound();
        }

        [HttpPost]
        [Route("user")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                Level = 3,
                JoinDate = DateTime.Now
            };

            IdentityResult addUserResult = await _userMgr.CreateAsync(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
                return BadRequest("Error during user creation");

            //send confirmation email
            var code = await _userMgr.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));
            await _userMgr.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            Uri location = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(location, user);
        }

        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await _userMgr.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
                return Ok();
            else
                return InternalServerError(new Exception("Error confirming email"));
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityResult result = await _userMgr.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
                return InternalServerError(new Exception("Error changing password"));

            return Ok();
        }

        [HttpDelete]
        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            var appUser = await _userMgr.FindByIdAsync(id);

            if (appUser == null)
                return NotFound();

            IdentityResult result = await _userMgr.DeleteAsync(appUser);

            if (!result.Succeeded)
                return InternalServerError(new Exception("Error deleting user"));

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign)
        {
            var appUser = await _userMgr.FindByIdAsync(id);

            if (appUser == null)
                return NotFound();

            var currentRoles = await _userMgr.GetRolesAsync(appUser.Id);
            var rolesNotExist = rolesToAssign.Except(_roleMgr.Roles.Select(x => x.Name)).ToArray();

            if (rolesNotExist.Count() > 0)
            {
                ModelState.AddModelError("", string.Format("Roles '{0}' does not exist", string.Join(",", rolesNotExist)));
                return BadRequest(ModelState);
            }

            var removeResult = await _userMgr.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray());

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return BadRequest(ModelState);
            }

            var addResult = await _userMgr.AddToRolesAsync(appUser.Id, rolesToAssign);

            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add user roles");
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}