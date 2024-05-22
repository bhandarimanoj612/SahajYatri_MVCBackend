using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;

namespace Sahaj_Yatri.Controllers
{
    // Controller for handling vehicle search requests
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleSearchController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        // Constructor injecting the application database context
        public VehicleSearchController(ApplicationDBContext db)
        {
            _db = db;
        }

        // Action method to perform vehicle search based on search criteria
        [HttpGet]
        public IActionResult VehicleSearch([FromQuery] Search searchCriteria)
        {
            try
            {
                // Start with base query
                var query = _db.Vehicles.AsQueryable();

                // Apply search criteria
                if (!string.IsNullOrEmpty(searchCriteria.Name))
                {
                    query = query.Where(v => v.Name.Contains(searchCriteria.Name));
                }
                if (!string.IsNullOrEmpty(searchCriteria.Location))
                {
                    query = query.Where(v => v.Location.Contains(searchCriteria.Location));
                }
               
                // Execute the query and find search results
                var searchResults = query.ToList();

                // Return search results
                return Ok(searchResults);
            }
            catch (Exception)
            {
                // Handle exceptions and return appropriate status code
                return StatusCode(500, "An error occurred while processing the search request.");
            }
        }
    }
}
