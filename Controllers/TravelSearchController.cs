// Importing necessary namespaces
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;

// Declaring the controller class
namespace Sahaj_Yatri.Controllers
{
    // Setting up routing and API controller attributes
    [Route("api/[controller]")]
    [ApiController]
    public class TravelSearchController : ControllerBase
    {
        // Declaring private fields
        private readonly ApplicationDBContext _db;

        // Constructor to initialize the database context
        public TravelSearchController(ApplicationDBContext db)
        {
            _db = db;
        }

        // Action method to handle GET requests for travel searches
        [HttpGet]
        public IActionResult TravelsSearch([FromQuery] Search searchCriteria)
        {
            try
            {
                // Start with the base query
                var query = _db.Travel.AsQueryable();

                // Apply search criteria
                if (!string.IsNullOrEmpty(searchCriteria.Name))
                {
                    query = query.Where(t => t.Name.Contains(searchCriteria.Name));
                }
                if (!string.IsNullOrEmpty(searchCriteria.Location))
                {
                    query = query.Where(t => t.Location.Contains(searchCriteria.Location));
                }

                // Execute the query and find search results
                var searchResults = query.ToList();

                // Return HTTP response with search results
                return Ok(searchResults);
            }
            catch (Exception)
            {
                // Handle exceptions and return appropriate HTTP status code
                return StatusCode(500, "An error occurred while processing the search request.");
            }
        }
    }
}
