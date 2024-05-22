using Microsoft.AspNetCore.Mvc;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelSearchController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public HotelSearchController(ApplicationDBContext db)
        {
            _db = db;
        }

        // Endpoint for searching hotels based on criteria
        [HttpGet]
        public IActionResult HotelSearch([FromQuery] Search searchCriteria)
        {
            try
            {
                // Start with base query
                var query = _db.Hotels.AsQueryable();

                // Apply search criteria
                if (!string.IsNullOrEmpty(searchCriteria.Name))
                {
                    query = query.Where(h => h.Name.Contains(searchCriteria.Name));
                }
                if (!string.IsNullOrEmpty(searchCriteria.Location))
                {
                    query = query.Where(h => h.Location.Contains(searchCriteria.Location));
                }

                // Execute the query and find search results
                var searchResults = query.ToList();

                // Return the search results
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
