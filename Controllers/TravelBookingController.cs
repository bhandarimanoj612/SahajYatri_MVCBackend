using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using Sahaj_Yatri.Models.Dto;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelBookingController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public TravelBookingController(ApplicationDBContext db)
        {
            _db = db;


        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TravelBooking>>> GetTravelBooking()
        {
            return await _db.TravelBookings.ToListAsync();
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TravelBooking>> GetTravelBooking(int id)
        {
            var travelBooking = await _db.TravelBookings.FindAsync(id);
            if (travelBooking == null)
            {
                return NotFound();
            }
            return travelBooking;
        }

        // GET: api/HotelBooking/username
        [HttpGet("user/{userName}")]
        public async Task<ActionResult<IEnumerable<TravelBooking>>> GetTravelBookingsByUserName(string userName)
        {
            var travelBookings = await _db.TravelBookings
                .Where(vb => vb.UserName == userName)
                .ToListAsync();

            if (travelBookings == null || !travelBookings.Any())
            {
                return NotFound("No bookings found for the specified user.");
            }

            return travelBookings;
        }

        [HttpPost]
        public async Task<ActionResult<TravelBooking>> CreateTravelBooking(TravelBookingCreateDTO travelBookingCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var travel = await _db.Travel.FirstOrDefaultAsync(x=>x.Name==travelBookingCreateDTO.Name);

            if (travel == null)
            {
                return BadRequest("invalid TravelPackage Name");
            }


            var travelBooking = new TravelBooking
            {
                
                UserName = travelBookingCreateDTO.UserName,
                PricePerDay = travelBookingCreateDTO.PricePerDay,
                TotalPrice = travelBookingCreateDTO.TotalPrice,
                Image=travelBookingCreateDTO.Image,
                StartDate = travelBookingCreateDTO.StartDate,
                EndDate = travelBookingCreateDTO.EndDate,
                StripePaymentIntentID = travelBookingCreateDTO.StripePaymentIntentID,
                Status = "Completed",
                NumberOfGuests = travelBookingCreateDTO.NumberOfGuests,
                Name = travelBookingCreateDTO.Name,
                NumberOfDays = travelBookingCreateDTO.NumberOfDays,
                Category = "TravelBooking" ,// Default value for Category
                Email = travelBookingCreateDTO.Email,
            };
            _db.TravelBookings.Add(travelBooking);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTravelBooking), new { id = travelBooking.Id }, travelBooking);

        }

        // PUT: api/travelBooking/
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTravelBooking(int id, TravelBooking travelBooking)
        {
            if (id != travelBooking.Id)
            {
                return BadRequest();
            }

            _db.Entry(travelBooking).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TravelBookExists(id))
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

        // DELETE: api/TravelBooking/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTravelBookng(int id)
        {
            var travelBooking = await _db.TravelBookings.FindAsync(id);
            if (travelBooking == null)
            {
                return NotFound();
            }

            _db.TravelBookings.Remove(travelBooking);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        private bool TravelBookExists(int id)
        {
            return _db.TravelBookings.Any(e => e.Id == id);
        }

       


    }
}