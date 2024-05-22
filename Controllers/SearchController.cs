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
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public SearchController(ApplicationDBContext db)
        {
            _db = db;
        }

        // Endpoint for searching all types of entities based on criteria
        [HttpGet]
        public IActionResult SearchAll([FromQuery] Search searchCriteria)
        {
            try
            {
                // Start with base queries for each entity type
                var hotelQuery = _db.Hotels.AsQueryable();
                var vehicleQuery = _db.Vehicles.AsQueryable();
                var travelQuery = _db.Travel.AsQueryable();

                // Apply search criteria for each entity type
                if (!string.IsNullOrEmpty(searchCriteria.Name))
                {
                    hotelQuery = hotelQuery.Where(h => h.Name.Contains(searchCriteria.Name));
                    vehicleQuery = vehicleQuery.Where(v => v.Name.Contains(searchCriteria.Name));
                    travelQuery = travelQuery.Where(t => t.Name.Contains(searchCriteria.Name));
                }
                if (!string.IsNullOrEmpty(searchCriteria.Location))
                {
                    hotelQuery = hotelQuery.Where(h => h.Location.Contains(searchCriteria.Location));
                    vehicleQuery = vehicleQuery.Where(v => v.Location.Contains(searchCriteria.Location));
                    travelQuery = travelQuery.Where(t => t.Location.Contains(searchCriteria.Location));
                }

                // Execute the queries and find search results
                var hotelSearchResults = hotelQuery.ToList();
                var vehicleSearchResults = vehicleQuery.ToList();
                var travelSearchResults = travelQuery.ToList();

                // Combine search results for all entities
                var searchResults = new
                {
                    Hotels = hotelSearchResults,
                    Vehicles = vehicleSearchResults,
                    Travel = travelSearchResults
                };

                // Return the combined search results
                return Ok(searchResults);
            }
            catch (Exception)
            {
                // Handle exceptions and return appropriate status code
                return StatusCode(500, "An error occurred while processing the search request.");
            }
        }
//all
        [HttpGet("all")]
        public IActionResult GetAllItems()
        {
            try
            {
                // Your logic to retrieve all items from each table
                var allHotels = _db.Hotels.ToList();
                var allVehicles = _db.Vehicles.ToList();
                var allTravel = _db.Travel.ToList();

                var allItems = new
                {
                    Hotels = allHotels,
                    Vehicles = allVehicles,
                    Travel = allTravel
                };

                return Ok(allItems);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving all items.");
            }
        }


        //for  getting popular and recommedation
        // Endpoint for getting popular items
        [HttpGet("popular")]
        public IActionResult GetPopularItems()
        {
            try
            {
                //  logic to retrieve popular items, for example:
                var popularHotels = _db.Hotels.OrderByDescending(h => h.Review).Take(5).ToList();
                var popularVehicles = _db.Vehicles.OrderByDescending(v => v.Review).Take(5).ToList();
                var popularTravel = _db.Travel.OrderByDescending(t => t.Review).Take(5).ToList();

                var popularItems = new
                {
                    Hotels = popularHotels,
                    Vehicles = popularVehicles,
                    Travel = popularTravel
                };

                return Ok(popularItems);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving popular items.");
            }
        }
        ///I've added an additional condition t.Review > 100 to filter the recommended items based on a review count 
        // Endpoint for getting recommended items

        // Endpoint for getting recommended items
        [HttpGet("recommendation")]
        public IActionResult GetRecommendedItems()
        {
            try
            {
                //  to retrieve recommended items, for example:
                var recommendedHotels = _db.Hotels.Where(h => h.Rating >= 4 && h.Review > 100).OrderByDescending(h => h.Rating).Take(5).ToList();
                var recommendedVehicles = _db.Vehicles.Where(v => v.Rating >= 4 && v.Review > 100).OrderByDescending(v => v.Rating).Take(5).ToList();
                var recommendedTravel = _db.Travel.Where(t => t.Rating >= 4 && t.Review > 100).OrderByDescending(t => t.Rating).Take(5).ToList();

                var recommendedItems = new
                {
                    Hotels = recommendedHotels,
                    Vehicles = recommendedVehicles,
                    Travel = recommendedTravel
                };

                return Ok(recommendedItems);
            }
            catch (Exception )
            {
                return StatusCode(500, "An error occurred while retrieving recommended items.");
            }
        }

        [HttpGet("email/{email}")]
        public IActionResult GetListsByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email cannot be null or empty");
            }

            // Fetch hotels based on the provided email
            var hotels = _db.Hotels.Where(h => h.Email == email).ToList();

            // Fetch travels based on the provided email
            var travels = _db.Travel.Where(t => t.Email == email).ToList();

            // Fetch vehicles based on the provided email
            var vehicles = _db.Vehicles.Where(v => v.Email == email).ToList();

            // Combine all lists into a single response object
            var lists = new
            {
                Hotels = hotels,
                Travels = travels,
                Vehicles = vehicles
            };

            if (lists.Hotels.Count == 0 && lists.Travels.Count == 0 && lists.Vehicles.Count == 0)
            {
                return NotFound("No lists found with the provided email");
            }

            return Ok(lists);
        }

    }
}
