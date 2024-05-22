
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public BookingController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: api/Booking/user/username
        // GET: api/Booking/user/username
        [HttpGet("user/{userName}")]
        public async Task<ActionResult<IEnumerable<object>>> GetBookingsByUserName([Required] string userName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var hotelBookings = await _db.HotelBookings.Where(hb => hb.UserName == userName).ToListAsync();
                var travelBookings = await _db.TravelBookings.Where(tb => tb.UserName == userName).ToListAsync();
                var vehicleBookings = await _db.VehicleBookings.Where(vb => vb.UserName == userName).ToListAsync();

                if (!hotelBookings.Any() && !travelBookings.Any() && !vehicleBookings.Any())
                {
                    return NotFound("No bookings found for the specified user.");
                }

                var bookings = new
                {
                    HotelBookings = hotelBookings,
                    TravelBookings = travelBookings,
                    VehicleBookings = vehicleBookings
                };

                return Ok(bookings);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving bookings.");
            }
        }

        // GET: api/Booking/user/admin/{adminEmail}
        [HttpGet("user/admin/{email}")]
        public async Task<ActionResult<IEnumerable<object>>> GetBookingsByAdminEmail(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var hotelBookings = await _db.HotelBookings.Where(hb => hb.Email == email).ToListAsync();
                var travelBookings = await _db.TravelBookings.Where(tb => tb.Email == email).ToListAsync();
                var vehicleBookings = await _db.VehicleBookings.Where(vb => vb.Email == email).ToListAsync();

                var bookings = new
                {
                    HotelBookings = hotelBookings,
                    TravelBookings = travelBookings,
                    VehicleBookings = vehicleBookings
                };

                if (!hotelBookings.Any() && !travelBookings.Any() && !vehicleBookings.Any())
                {
                    return NotFound("No bookings found for the specified admin email.");
                }

                return Ok(bookings);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving bookings.");
            }
        }

        // Other CRUD operations for bookings (create, update, delete) can be added here
    }
}
