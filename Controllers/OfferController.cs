using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using Sahaj_Yatri.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sahaj_Yatri.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Sahaj_Yatri.Models.Dto.Role;
namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        private readonly IFileService _fileService;
        public OfferController(ApplicationDBContext db, IFileService fileService)
        {
            _db = db;
            _fileService = fileService;
        }

        // GET: api/Offer
    
        [HttpGet]
        //[Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<IEnumerable<Offer>>> GetOffers()
        {
            return await _db.Offers.ToListAsync();
        }

        // GET: api/Offer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Offer>> GetOffer(int id)
        {
            var offer = await _db.Offers.FindAsync(id);

            if (offer == null)
            {
                return NotFound();
            }

            return offer;
        }

        [HttpPost]
        public async Task<ActionResult<Offer>> CreateOffer([FromForm] OfferCreateDTO  offerCreateDto, IFormFile file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Handle file upload
                    Offer offerCreate = new()
                    {
                        Title = offerCreateDto.Title,
                        ImageUrl = await _fileService.WriteFile(file),
                        Description = offerCreateDto.Description

                    };
                    _db.Offers.Add(offerCreate);
                    await _db.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetOffer), new { id = offerCreate.Id }, offerCreate);
                }
                else
                {
                    // Handle invalid model state
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOffer(int id, [FromForm] OfferCreateDTO offerCreateDto, IFormFile file)
        {
            try
            {
                var offer = await _db.Offers.FindAsync(id);
                if (offer == null)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    // Handle file upload
                    if (file != null)
                    {
                        offer.ImageUrl = await _fileService.WriteFile(file);
                    }

                    // Update offer properties
                    offer.Title = offerCreateDto.Title;
                    offer.Description = offerCreateDto.Description;

                    _db.Entry(offer).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    return NoContent();
                }
                else
                {
                    // Handle invalid model state
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, ex.Message);
            }
        }



        // DELETE: api/Offer/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Offer>> DeleteOffer(int id)
        {
            var offer = await _db.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }

            _db.Offers.Remove(offer);
            await _db.SaveChangesAsync();

            return offer;
        }

        private bool OfferExists(int id)
        {
            return _db.Offers.Any(e => e.Id == id);
        }
    }
}
