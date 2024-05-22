using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using Sahaj_Yatri.Models.Dto;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelBookingController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public HotelBookingController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: api/HotelBooking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelBooking>>> GetHotelBookings()
        {
            return await _db.HotelBookings.ToListAsync();
        }

        // GET: api/HotelBooking/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<HotelBooking>> GetHotelBooking(int id)
        {
            var hotelBooking = await _db.HotelBookings.FindAsync(id);

            if (hotelBooking == null)
            {
                return NotFound();
            }

            return hotelBooking;
        }
        //getting booking details for admin based on hotel name
        [HttpGet("hotel/{hotelName}")]
        public async Task<ActionResult<IEnumerable<HotelBooking>>> GetHotelBookingsByHotelName(string hotelName)
        {
            var hotelBookings = await _db.HotelBookings
                .Where(vb => vb.Name == hotelName)
                .ToListAsync();

            if (hotelBookings == null || !hotelBookings.Any())
            {
                return NotFound("No bookings found for the specified hotel name.");
            }

            return hotelBookings;
        }
// GET: api/HotelBooking/hotel/email
[HttpGet("hotel/email/{email}")]
public async Task<ActionResult<IEnumerable<HotelBooking>>> GetBookingByEmail(string email)
{
    var hotelBookings = await _db.HotelBookings
        .Where(vb => vb.Email == email)
        .ToListAsync();

    if (hotelBookings == null || !hotelBookings.Any())
    {
        return NotFound("No bookings found for the specified hotel email.");
    }

    return hotelBookings;
}



        // GET: api/HotelBooking/username
        [HttpGet("user/{userName}")]
        public async Task<ActionResult<IEnumerable<HotelBooking>>> GetHotelBookingsByUserName(string userName)
        {
            var hotelBookings = await _db.HotelBookings
                .Where(vb => vb.UserName == userName)
                .ToListAsync();

            if (hotelBookings == null || !hotelBookings.Any())
            {
                return NotFound("No bookings found for the specified user.");
            }

            return hotelBookings;
        }


        // POST: api/HotelBooking
        [HttpPost]
        public async Task<ActionResult<HotelBooking>> CreateHotelBooking(HotelBookingCreateDTO hotelBookingDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the hotel by name
            var hotel = await _db.Hotels.FirstOrDefaultAsync(x => x.Name == hotelBookingDTO.Name);
            if (hotel == null)
            {
                return BadRequest("Invalid hotel name");
            }

            var hotelBooking = new HotelBooking
            {
                // Assign the hotel name instead of HotelId
                Name = hotelBookingDTO.Name,
                Image= hotelBookingDTO.Image,
                UserName = hotelBookingDTO.UserName,
                StartDate = hotelBookingDTO.StartDate,
                EndDate = hotelBookingDTO.EndDate,
                NumberOfGuests = hotelBookingDTO.NumberOfGuests,
                TotalPrice = hotelBookingDTO.TotalPrice,
                PricePerDay = hotelBookingDTO.PricePerDay, 
                StripePaymentIntentID = hotelBookingDTO.StripePaymentIntentID,
                Status = "Completed", // Default status
                NumberOfDays= hotelBookingDTO.NumberOfDays,
                Category = "HotelBooking",
                Email = hotelBookingDTO.Email,
            };
            _db.HotelBookings.Add(hotelBooking);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHotelBooking), new { id = hotelBooking.Id }, hotelBooking);
        }

        // PUT: api/HotelBooking/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotelBooking(int id, HotelBooking hotelBooking)
        {
            if (id != hotelBooking.Id)
            {
                return BadRequest();
            }

            _db.Entry(hotelBooking).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelBookingExists(id))
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

        // DELETE: api/HotelBooking/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotelBooking(int id)
        {
            var hotelBooking = await _db.HotelBookings.FindAsync(id);
            if (hotelBooking == null)
            {
                return NotFound();
            }

            _db.HotelBookings.Remove(hotelBooking);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool HotelBookingExists(int id)
        {
            return _db.HotelBookings.Any(e => e.Id == id);
        }

  
    }

}

