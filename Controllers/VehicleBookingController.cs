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
    public class VehicleBookingController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        public VehicleBookingController(ApplicationDBContext db)
        {
            _db = db;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleBooking>>> GetVehicleBookings()
        {
            return await _db.VehicleBookings.ToListAsync();

           
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VehicleBooking>> GetVehicleBooking(int id)
        {
            var vehicleBooking = await _db.VehicleBookings.FindAsync(id);
            if (vehicleBooking == null)
            {
                return NotFound();
            }
            return vehicleBooking;
        }
        //getting vehicle booking by username 
        [HttpGet("user/{userName}")]
        public async Task<ActionResult<IEnumerable<VehicleBooking>>> GetVehicleBookingsByUserName(string userName)
        {
            var vehicleBookings = await _db.VehicleBookings
                .Where(vb => vb.UserName == userName)
                .ToListAsync();

            if (vehicleBookings == null || !vehicleBookings.Any())
            {
                return NotFound("No bookings found for the specified user.");
            }

            return vehicleBookings;
        }

        [HttpPost]
        public async Task<ActionResult<VehicleBooking>> CreateVehicleBooking(VehicleBookingCreateDTO vehicleBookingCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var vehicle = await _db.Vehicles.FirstOrDefaultAsync(x => x.Name == vehicleBookingCreateDTO.Name);

            if (vehicle == null)
            {
                return BadRequest("invalid vehicle ID");
            }

           

            var vehicleBooking = new VehicleBooking
            {
                UserName = vehicleBookingCreateDTO.UserName,
                PricePerDay = vehicleBookingCreateDTO.PricePerDay,
                TotalPrice = vehicleBookingCreateDTO.TotalPrice,
                Image= vehicleBookingCreateDTO.Image,
                StartDate = vehicleBookingCreateDTO.StartDate,
                EndDate = vehicleBookingCreateDTO.EndDate,
                StripePaymentIntentID = vehicleBookingCreateDTO.StripePaymentIntentID,
                Status = "Completed",
                NumberOfGuests = vehicleBookingCreateDTO.NumberOfGuests,
                Name = vehicleBookingCreateDTO.Name,
                NumberOfDays = vehicleBookingCreateDTO.NumberOfDays,
                Category = "VehicleBooking", // Default value for Category
                Email = vehicleBookingCreateDTO.Email,
            };

            _db.VehicleBookings.Add(vehicleBooking);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVehicleBooking), new { id = vehicleBooking.Id }, vehicleBooking);
        }
        // PUT: api/HotelBooking/
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicleBooking(int id, VehicleBooking vehicleBooking)
        {
            if (id != vehicleBooking.Id)
            {
                return BadRequest();
            }

            _db.Entry(vehicleBooking).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleBookExists(id))
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

        // DELETE: api/VehicleBooking/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicleBookng(int id)
        {
            var vehicleBooking = await _db.VehicleBookings.FindAsync(id);
            if (vehicleBooking == null)
            {
                return NotFound();
            }

            _db.VehicleBookings.Remove(vehicleBooking);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool VehicleBookExists(int id)
            {
                return _db.VehicleBookings.Any(e => e.Id == id);
            }

       
         }
    }
