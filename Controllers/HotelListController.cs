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
    [Route("api/HotelList")]
    [ApiController]
    public class HotelListController : ControllerBase
    {
        
        private readonly ApplicationDBContext _db;
        private ApiResponse _response;
        private readonly IFileService _fileService;
        //constructer
        public HotelListController(ApplicationDBContext db,IFileService fileService)
        { 
            _db = db;
            _response = new ApiResponse();
            _fileService = fileService;

        }
        //below line of code is used for uploading image in codde with below code i am making image upload 
     

        //popular 


        [HttpGet("popular")]
        public IActionResult GetPopularHotels()
        {
            // Logic to fetch popular hotels (e.g., based on ratings, bookings, etc.)
            var popularHotels = _db.Hotels.OrderByDescending(h => h.Rating).Take(7).ToList();
            return Ok(popularHotels);
        }
        //recommendded

        [HttpGet("recommend")]
        public IActionResult GetRecommendedHotels()
        {
            // Logic to fetch recommended hotels (e.g., based on user preferences, trending, etc.)
            var recommendedHotels = _db.Hotels.OrderBy(h => h.Price).Take(7).ToList();
            return Ok(recommendedHotels);
        }

        [HttpGet]
        //[Authorize(Roles = Role.Admin)]
        //[Authorize(Roles = Role.Hotel +"," + Role.Admin)]
        public IActionResult GetHotelList()
        {
            _response.Result = _db.Hotels;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);//it will go to database and fetch all database and return it back

        }


        //getting all individual list of hotel items
        [HttpGet("{id:int}",Name ="GetHotelList")]

        //[Authorize(Roles = Role.Customer + "," + Role.Admin + "," + Role.Hotel)]
        public async Task<IActionResult> GetHotelList(int id )
        {
             if ( id == 0 )
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            //getting individual id 
            Hotel hotel =await _db.Hotels.FirstOrDefaultAsync(h => h.Id == id);
            if( hotel == null )
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            _response.Result = hotel;//if the hotel item is not null hotell is result
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);//it will go to database and fetch all database and return it back
        }
        //get hotel besed  on email address
       // Other existing methods...

            // GET: api/HotelList/email/{email}
            [HttpGet("email/{email}")]
            public IActionResult GetHotelsByEmail(string email)
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email cannot be null or empty");
                }

                // Fetch hotels based on the provided email
                var hotels = _db.Hotels.Where(h => h.Email == email).ToList();

                if (hotels.Count == 0)
                {
                    return NotFound("No hotels found with the provided email");
                }

                return Ok(hotels);
            }

        //post api
        [HttpPost]
     
        [Authorize(Roles = Role.Hotel + "," + Role.Admin)]

        public async Task<ActionResult<ApiResponse>> CreateHotelList([FromForm]HotelListCreateDto  hotelListCreateDTO, IFormFile file)
        {
            //from form is used because we also upload image when creating hotel list

            try
            {
                if(ModelState.IsValid)//modelt state will check required filed in model are valid or not 
                {
                   
                    Hotel hotelListToCreate = new()
                    {
                        Name = hotelListCreateDTO.Name,
                        Price = hotelListCreateDTO.Price,
                        Email = hotelListCreateDTO.Email,
                        Image = await _fileService.WriteFile(file),
                        PhoneNumber = hotelListCreateDTO.PhoneNumber,
                        Rating = hotelListCreateDTO.Rating,
                        Location = hotelListCreateDTO.Location,
                        Review = hotelListCreateDTO.Review,
                        LongDescription = hotelListCreateDTO.LongDescription,
                        ShortDescription = hotelListCreateDTO.ShortDescription,
                        Category = "HotelBooking" // Default value for HotelCategory

                    };

                    _db.Add(hotelListToCreate);
                    await _db.SaveChangesAsync();
                    _response.Result = hotelListToCreate;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetHotelList", new { id = hotelListToCreate.Id }, _response);

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
        [Authorize(Roles = Role.Hotel + "," + Role.Admin)]
        public async Task<ActionResult<ApiResponse>> UpdateHotelList(int id ,[FromForm] HotelListUpdateDto hotelListUpdateDto, IFormFile file)
        {
            //from form is used because we also upload image when creating hotel list

            try
            {
                if (ModelState.IsValid)//modelt state will check required filed in model are valid or not 
                {
                    
                    if(hotelListUpdateDto == null || id != hotelListUpdateDto.Id)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }

                    //Hotel hotelListFromDb = await _db.Hotels.FirstOrDefaultAsync(h => h.Id == id);  or below
                    Hotel hotelListFromDb = await _db.Hotels.FindAsync(id);
                    
                    if(hotelListFromDb == null)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    hotelListFromDb.Name = hotelListUpdateDto.Name;
                    hotelListFromDb.Review = hotelListUpdateDto.Review;
                    hotelListFromDb.PhoneNumber = hotelListUpdateDto.PhoneNumber;
                    hotelListFromDb.Rating = hotelListUpdateDto.Rating;
                    hotelListFromDb.Email = hotelListUpdateDto.Email;
                    hotelListFromDb.ShortDescription = hotelListUpdateDto.ShortDescription;
                    hotelListFromDb.LongDescription = hotelListUpdateDto.LongDescription;
                    hotelListFromDb.Location = hotelListUpdateDto.Location;
                    hotelListFromDb.Price = hotelListUpdateDto.Price;
                    hotelListFromDb.Image = await _fileService.WriteFile(file);
                    //if i upload image from file  i need to check image condition
                    _db.Update(hotelListFromDb);
                    _db.SaveChanges();
                    _response.StatusCode = HttpStatusCode.NoContent;
                    return Ok( _response);

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

        //http delete

        //put 
      
        [HttpDelete("{id:int}")]
        //[Authorize(Roles = Role.Hotel + "," + Role.Admin)]
        public async Task<ActionResult<ApiResponse>> DeleteHotelList(int id)
        {
            //from form is used because we also upload image when creating hotel list

            try
            {
               
                    if ( id == 0)
                    {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                    }

                    //Hotel hotelListFromDb = await _db.Hotels.FirstOrDefaultAsync(h => h.Id == id);  or below
                    Hotel hotelListFromDb = await _db.Hotels.FindAsync(id);

                    if (hotelListFromDb == null)
                    {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                    }


                // for adding millisecond delay to the code when we are calling it 
                int millisecond = 2000;
                Thread.Sleep(millisecond);

                    _db.Remove(hotelListFromDb);
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
