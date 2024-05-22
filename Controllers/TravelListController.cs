using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using Sahaj_Yatri.Models.Dto;
using Sahaj_Yatri.Models.Dto.Role;
using Sahaj_Yatri.Services;
using System.Net;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/TravelList")]
    [ApiController]
    public class TravelListController : ControllerBase
    {

        //For calling databases
        private readonly ApplicationDBContext _db;
        private ApiResponse _response;
       
        //for making file upload in travel  for image upload
        private readonly IFileService _fileService;
        public TravelListController(ApplicationDBContext db, IFileService fileService)
        {
            _db = db;
            _response = new ApiResponse();
            _fileService = fileService;
        }


        //popular recommended 
        [HttpGet("popular")]
        public IActionResult GetPopularTravels()
        {
            // Logic to fetch popular travel items (e.g., based on ratings, bookings, etc.)
            var popularTravels = _db.Travel.OrderByDescending(t => t.Rating).Take(5).ToList();
            return Ok(popularTravels);
        }

        [HttpGet("recommend")]
        public IActionResult GetRecommendedTravels()
        {
            // Logic to fetch recommended travel items (e.g., based on user preferences, trending, etc.)
            var recommendedTravels = _db.Travel.OrderBy(t => t.Price).Take(5).ToList();
            return Ok(recommendedTravels);
        }



        //for  file upload 



        [HttpGet]
        public IActionResult GetTravelList()
        {
            _response.Result = _db.Travel;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }
        //for calling vehicle list by id 
        [HttpGet("{id:int}", Name = "GetTravelList")]
        public async Task<IActionResult> GetTravelList(int id)
        {
            if (id == 0)
            {
                _response.Result = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);

            }
            Travel travel = await _db.Travel.FirstOrDefaultAsync(u => u.Id == id);
            //below logic is run when there is no items in the vehcile databases
            if (travel == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            _response.Result = travel;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

        //post api
        [HttpPost]
        [Authorize(Roles = Role.Travel + "," + Role.Admin)]

        //public async Task<ActionResult<ApiResponse>> CreateTraveleList([FromForm] TravelCreateDto travelCreateDto, IFormFile file)
        public async Task<ActionResult<ApiResponse>> CreateTraveleList([FromForm] TravelCreateDto travelCreateDto, IFormFile file)
        {
            //from form is used because we also upload image when creating hotel list

            try
            {
                if (ModelState.IsValid)//modelt state will check required filed in model are valid or not 
                {
                    
                    Travel travelListToCreate = new()
                    {
                        Name = travelCreateDto.Name,
                        Price = travelCreateDto.Price,
                        Email = travelCreateDto.Email,
                        Image = await _fileService.WriteFile(file),
                        //Image = travelCreateDto.Image,
                        PhoneNumber = travelCreateDto.PhoneNumber,
                        Rating = travelCreateDto.Rating,
                        Location = travelCreateDto.Location,
                        Review = travelCreateDto.Review,
                        LongDescription = travelCreateDto.LongDescription,
                        ShortDescription = travelCreateDto.ShortDescription,
                        Category = "TravelBooking" // Default value for Category
                    };

                    _db.Add(travelListToCreate);
                    await _db.SaveChangesAsync();
                    _response.Result = travelListToCreate;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetTravelList", new { id = travelListToCreate.Id }, _response);

                }
                else
                {
                    _response.IsSuccess = false;

                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;


        }
        //put 
        [HttpPut("{id:int}")]
        [Authorize(Roles = Role.Travel + "," + Role.Admin)]
        public async Task<ActionResult<ApiResponse>> UpdateTravelList(int id, [FromForm] TravelUpdateDto travelUpdateDto, IFormFile file)
        {
            //from form is used because we also upload image when creating hotel list

            try
            {
                if (ModelState.IsValid)//modelt state will check required filed in model are valid or not 
                {

                    if (travelUpdateDto == null || id != travelUpdateDto.Id)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }

                    //vehicle vehicleistFromDb = await _db.Hotels.FirstOrDefaultAsync(h => h.Id == id);  or below
                     Travel travelListFromDb = await _db.Travel.FindAsync(id);

                    if (travelListFromDb == null)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    travelListFromDb.Name = travelUpdateDto.Name;
                    travelListFromDb.Review = travelUpdateDto.Review;
                    travelListFromDb.PhoneNumber = travelUpdateDto.PhoneNumber;
                    travelListFromDb.Rating = travelUpdateDto.Rating;
                    travelListFromDb.Email = travelUpdateDto.Email;
                    travelListFromDb.ShortDescription = travelUpdateDto.ShortDescription;
                    travelListFromDb.LongDescription = travelUpdateDto.LongDescription;
                    travelListFromDb.Location = travelUpdateDto.Location;
                    travelListFromDb.Price = travelUpdateDto.Price;
                    //travelListFromDb.Image = travelUpdateDto.Image;
                    travelListFromDb.Image = await _fileService.WriteFile(file);

                    //if i upload image from file  i need to check image condition


                    _db.Update(travelListFromDb);
                    _db.SaveChanges();
                    _response.StatusCode = HttpStatusCode.NoContent;
                    return Ok(_response);

                }
                else
                {
                    _response.IsSuccess = false;

                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }


        [HttpDelete("{id:int}")]
        //[Authorize(Roles = Role.Hotel + "," + Role.Admin)]
        public async Task<ActionResult<ApiResponse>> DeleteTravelList(int id)
        {
            //from form is used because we also upload image when creating hotel list

            try
            {

                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                }

                //Hotel hotelListFromDb = await _db.Hotels.FirstOrDefaultAsync(h => h.Id == id);  or below
                Travel travelListFromDb = await _db.Travel.FindAsync(id);

                if (travelListFromDb == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                }


                // for adding millisecond delay to the code when we are calling it 
                int millisecond = 2000;
                Thread.Sleep(millisecond);

                _db.Remove(travelListFromDb);
                _db.SaveChanges();
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);



            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }


    }


}


