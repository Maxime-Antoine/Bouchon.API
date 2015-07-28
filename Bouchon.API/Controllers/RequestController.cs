using Bouchon.API.BindingModels;
using Bouchon.API.Db;
using Bouchon.API.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bouchon.API.Controllers
{
    [RoutePrefix("api")]
    public class RequestController : ApiController
    {
        private IRepository<Request> _requestRepo;

        public RequestController(IRepository<Request> requestRepo)
        {
            _requestRepo = requestRepo;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var req = await _requestRepo.GetAll();

            return Ok(req);
        }

        [HttpGet]
        [Route("", Name = "GetRequestById")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var req = await _requestRepo.GetById(id);

            if (req == null)
                return NotFound();

            return Ok(req);
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(RequestBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var request = new Request
            {
                Category = model.Category,
                Ccy = model.Ccy,
                CityToBuy = model.CityToBuy,
                CreationDate = DateTime.Now,
                ItemDescription = model.ItemDescription,
                ItemName = model.ItemName,
                Picture = model.Picture,
                PreferedDealingLocation = model.PreferedDealingLocation,
                ProposedPrice = model.ProposedPrice,
                Status = ERequestStatus.Open,
                Url = model.Url,
                UserId = User.Identity.GetUserId()
            };

            var createdReq = await _requestRepo.Add(request);

            Uri location  = new Uri(Url.Link("GetRequestById", new { id = createdReq.UserId }));

            return Created(location, request);
        }
    }
}