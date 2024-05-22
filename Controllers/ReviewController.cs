using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ReviewController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Review
        [HttpGet]
        public ActionResult<IEnumerable<Review>> GetReviews()
        {
            return _context.Reviews.ToList();
        }

        // GET: api/Review/5
        [HttpGet("{id}")]
        public ActionResult<Review> GetReview(int id)
        {
            var review = _context.Reviews.Find(id);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        // POST: api/Review
        [HttpPost]
        public ActionResult<Review> PostReview(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();

            UpdateBookingData(review); // Update the corresponding booking data
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }

        // PUT: api/Review/5
        [HttpPut("{id}")]
        public IActionResult PutReview(int id, Review review)
        {
            if (id != review.Id)
            {
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;
            _context.SaveChanges();

            UpdateBookingData(review); // Update the corresponding booking data
            return NoContent();
        }

        // DELETE: api/Review/5
        [HttpDelete("{id}")]
        public ActionResult<Review> DeleteReview(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review == null)
            {
                return NotFound();
            }

            review.isDeleted = true; // Soft delete by setting the isDeleted property to true
            _context.SaveChanges();

            UpdateBookingData(review); // Update the corresponding booking data
            return review;
        }

        // Method to update the corresponding booking data based on the category
        private void UpdateBookingData(Review review)
        {
            switch (review.Category)
            {
                case "HotelBooking":
                    UpdateHotelData(review.Name);
                    break;
                case "TravelBooking":
                    UpdateTravelData(review.Name);
                    break;
                case "VehicleBooking":
                    UpdateVehicleData(review.Name);
                    break;
                default:
                    // Handle unknown category
                    break;
            }
        }

        // Method to update the hotel data
        private void UpdateHotelData(string hotelName)
        {
            var hotel = _context.Hotels.FirstOrDefault(h => h.Name == hotelName);
            if (hotel != null)
            {
                var ratings = _context.Reviews.Where(r => r.Name == hotelName).Select(r => r.Rating);
                if (ratings.Any())
                {
                    hotel.Rating = (int)ratings.Average();
                    hotel.Review = ratings.Count();
                    _context.SaveChanges();
                }
            }
        }

        // Method to update the travel data
        private void UpdateTravelData(string travelName)
        {
            // Implement logic to update travel data based on the review
            var hotel = _context.Travel.FirstOrDefault(h => h.Name == travelName);
            if (hotel != null)
            {
                var ratings = _context.Reviews.Where(r => r.Name == travelName).Select(r => r.Rating);
                if (ratings.Any())
                {
                    hotel.Rating = (int)ratings.Average();
                    hotel.Review = ratings.Count();
                    _context.SaveChanges();
                }
            }

        }

        // Method to update the vehicle data
        private void UpdateVehicleData(string vehicleName)
        {
            // Implement logic to update vehicle data based on the review
            var hotel = _context.Vehicles.FirstOrDefault(h => h.Name == vehicleName);
            if (hotel != null)
            {
                var ratings = _context.Reviews.Where(r => r.Name == vehicleName).Select(r => r.Rating);
                if (ratings.Any())
                {
                    hotel.Rating = (int)ratings.Average();
                    hotel.Review = ratings.Count();
                    _context.SaveChanges();
                }
            }
        }

        // GET: api/Review/UserReviews/{userName}
        [HttpGet("UserReviews/{userName}")]
        public ActionResult<IEnumerable<Review>> GetUserReviews(string userName)
        {
            var userReviews = _context.Reviews.Where(r => r.UserName == userName).ToList();
            return userReviews;
        }

        // GET: api/Review/AverageRating/{hotelName}
        [HttpGet("AverageRating/{hotelName}")]
        public ActionResult<double> GetAverageRating(string hotelName)
        {
            var ratings = _context.Reviews.Where(r => r.Name == hotelName).Select(r => r.Rating);
            if (ratings.Any())
            {
                return ratings.Average();
            }
            return 0;
        }

        // GET: api/Review/TotalReviews/{hotelName}
        [HttpGet("TotalReviews/{hotelName}")]
        public ActionResult<int> GetTotalReviews(string hotelName)
        {
            var totalReviews = _context.Reviews.Count(r => r.Name == hotelName);
            return totalReviews;
        }

        // GET: api/Review/CommentsByHotel/{hotelName}
        [HttpGet("CommentsByHotel/{hotelName}")]
        public ActionResult<IEnumerable<Review>> GetCommentsByHotel(string hotelName)
        {
            var comments = _context.Reviews.Where(r => r.Name == hotelName && !r.isDeleted).ToList();
            return comments;
        }
    }
}
