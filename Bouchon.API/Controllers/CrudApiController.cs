using Bouchon.API.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bouchon.API.Controllers
{
    public abstract class CrudApiController<T> : ApiController where T : class
    {
        protected IRepository<T> _repo;

        public CrudApiController(IRepository<T> repo)
        {
            _repo = repo;
        }

        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var res = await _repo.GetAll();

            return Ok(_repo);
        }

        [Route("{id:int}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var res = await _repo.GetById(id);

            if (res == null)
                return NotFound();
            else
                return Ok(res);
        }

        [Route("")]
        public async Task<IHttpActionResult> Post(T transaction)
        {
            if (ModelState.IsValid)
            {
                await _repo.Add(transaction);
                return Ok(transaction);
            }

            return BadRequest(ModelState);
        }

        [Route("")]
        public async Task<IHttpActionResult> Put(T transaction)
        {
            if (ModelState.IsValid)
            {
                await _repo.Update(transaction);
                return Ok(transaction);
            }

            return BadRequest(ModelState);
        }

        [Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            await _repo.Delete(id);

            return Ok(id);
        }
    }
}
