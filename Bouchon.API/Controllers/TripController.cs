using Bouchon.API.BindingModels;
using Bouchon.API.Db;
using Bouchon.API.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bouchon.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/trip")]
    public class TripController : ApiController
    {
        private IRepository<Trip> _tripRepo;

        public TripController(IRepository<Trip> tripRepo)
        {
            _tripRepo = tripRepo;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var trips = await _tripRepo.GetAll();

            return Ok(trips);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetTripById")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var trip = await _tripRepo.GetById(id);

            if (trip == null)
                return NotFound();

            return Ok(trip);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> GetForUser(string id)
        {
            var trips = await _tripRepo.Query()
                                      .Where(t => t.UserId == id)
                                      .ToListAsync();
            return Ok(trips);
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CreateTripBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var trip = new Trip
            {
                CreationDate = DateTime.Now,
                DepartureDate = model.DepartureDate,
                FromCity = model.FromCity,
                ReturnDate = model.ReturnDate,
                ToCity = model.ToCity,
                UserId = User.Identity.GetUserId()
            };

            var createdTrip = await _tripRepo.Add(trip);

            var url = new Uri(Url.Link("GetTripById", new { id = createdTrip.Id }));

            return Created(url, createdTrip);
        }
    }
}