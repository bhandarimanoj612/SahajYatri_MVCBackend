using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using Sahaj_Yatri.Models.Dto;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public FavoriteController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: api/Favorite
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetFavorites()
        {
            return await _db.Favorites.ToListAsync();
        }

        // GET: api/Favorite/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Favorite>> GetFavorite(int id)
        {
            var favorite = await _db.Favorites.FindAsync(id);

            if (favorite == null)
            {
                return NotFound();
            }

            return favorite;
        }

        // GET: api/Favorite/user/{email}
        [HttpGet("user/{email}")]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetFavoritesByUserEmail(string email)
        {
            // Retrieve favorites based on the provided email
            var favorites = await _db.Favorites.Where(f => f.Email == email && !f.IsDeleted).ToListAsync();

            if (favorites == null || favorites.Count == 0)
            {
                return NotFound("No favorites found for the specified email.");
            }

            return favorites;
        }
        // POST: api/Favorite
        [HttpPost]
        public async Task<ActionResult<Favorite>> CreateFavorite(FavoriteCreateDto favoriteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var favorite = new Favorite
            {
                Name = favoriteDto.Name,
                ShortDescription = favoriteDto.ShortDescription,
                LongDescription = favoriteDto.LongDescription,
                Price = favoriteDto.Price,
                Rating = favoriteDto.Rating,
                Review = favoriteDto.Review,
                Image=favoriteDto.Image,
                Email = favoriteDto.Email,
                PhoneNumber = favoriteDto.PhoneNumber,
                Location = favoriteDto.Location,
                Category = favoriteDto.Category,
                IsDeleted = false,
            };

            _db.Favorites.Add(favorite);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFavorite), new { id = favorite.Id }, favorite);
        }

        // PUT: api/Favorite/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFavorite(int id, FavoriteUpdate favoriteUpdate)
        {
            if (id != favoriteUpdate.Id)
            {
                return BadRequest();
            }

            var favorite = await _db.Favorites.FindAsync(id);
            if (favorite == null)
            {
                return NotFound();
            }

            favorite.Name = favoriteUpdate.Name;
            favorite.ShortDescription = favoriteUpdate.ShortDescription;
            favorite.LongDescription = favoriteUpdate.LongDescription;
            favorite.Price = favoriteUpdate.Price;
            favorite.Rating = favoriteUpdate.Rating;
            favorite.Review = favoriteUpdate.Review;
            favorite.Email = favoriteUpdate.Email;
            favorite.PhoneNumber = favoriteUpdate.PhoneNumber;
            favorite.Location = favoriteUpdate.Location;
            favorite.Category = favoriteUpdate.Category;
            favorite.Image = favoriteUpdate.Image;
            favorite.IsDeleted = false;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FavoriteExists(id))
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

        // DELETE: api/Favorite/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var favorite = await _db.Favorites.FindAsync(id);
            if (favorite == null)
            {
                return NotFound();
            }

            favorite.IsDeleted = true; // Mark favorite as deleted
            _db.Favorites.Update(favorite);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Favorite/{name}/{email}
        [HttpDelete("{name}/{email}")]
        public async Task<IActionResult> DeleteFavorite(string name, string email)
        {
            try
            {
                // Find the favorite based on hotel name and user email
                var favorite = await _db.Favorites.FirstOrDefaultAsync(f => f.Name == name && f.Email == email && !f.IsDeleted);

                if (favorite == null)
                {
                    return NotFound();
                }

                favorite.IsDeleted = true; // Mark favorite as deleted
                _db.Favorites.Update(favorite);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }



        private bool FavoriteExists(int id)
        {
            return _db.Favorites.Any(e => e.Id == id);
        }
    }
}
