//using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetTripController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        private readonly ApiResponse _response;
        public BudgetTripController(ApplicationDBContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Trip>>> GetTrips()
        {
            // Filter out soft deleted trips
            var trips = await _db.Trips.Where(t => !t.IsDeleted).ToListAsync();
            return trips;
        }

        // GET: api/Trips/5
        [HttpGet("{id}", Name = "GetTrip")]
        public async Task<ActionResult<Trip>> GetTrip(int id)
        {
            // Retrieve trip only if it's not soft deleted
            var trip = await _db.Trips.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (trip == null)
            {
                return NotFound();
            }
            return trip;
        }

        // GET: api/Trips/user/{userName}
        [HttpGet("user/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Trip>>> GetTripsByUserName(string userName)
        {
            // Filter out soft deleted trips
            var trips = await _db.Trips.Where(t => t.UserName == userName && !t.IsDeleted).ToListAsync();
            if (trips == null || !trips.Any())
            {
                return NotFound("No trips found for the specified user.");
            }
            return trips;
        }

        // PUT: api/Trips/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrip(int id, Trip trip)
        {
            if (id != trip.Id)
            {
                return BadRequest();
            }

            _db.Entry(trip).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Trips
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> AddTrip([FromBody] Trip trip)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Set the date to the current date
                    trip.Date = DateTime.Now;
                    _db.Trips.Add(trip);
                    await _db.SaveChangesAsync();
                    _response.Result = trip;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetTrip", new { id = trip.Id }, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return _response;
            }
        }

        // DELETE: api/Trips/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var trip = await _db.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            // Soft delete the trip
            trip.IsDeleted = true;
            _db.Trips.Update(trip);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool TripExists(int id)
        {
            return _db.Trips.Any(e => e.Id == id);
        }
    }
}
