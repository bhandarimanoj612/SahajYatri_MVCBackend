using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using Sahaj_Yatri.Models.Dto;
using Sahaj_Yatri.Models.Dto.Role;
using Sahaj_Yatri.Services;
using System.Net;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/VehicleList")]
    [ApiController]
    public class VehicleListController : ControllerBase
    {
        //For calling databases
        private readonly ApplicationDBContext _db;
        private ApiResponse _response;
        private readonly IFileService _fileService;

        //for making file upload image upload from system

        public VehicleListController(ApplicationDBContext db,IFileService fileService)
        {
            _db = db;
            _response = new ApiResponse();
            _fileService = fileService;
        }
        //for  file upload 


        //popular vehicle 
        [HttpGet("popular")]
        public IActionResult GetPopularVehicles()
        {
            // Logic to fetch popular vehicles (e.g., based on ratings, bookings, etc.)
            var popularVehicles = _db.Vehicles.OrderByDescending(v => v.Rating).Take(5).ToList();
            return Ok(popularVehicles);
        }

        //recommended
        [HttpGet("recommend")]
        public IActionResult GetRecommendedVehicles()
        {
            // Logic to fetch recommended vehicles (e.g., based on user preferences, trending, etc.)
            var recommendedVehicles = _db.Vehicles.OrderBy(v => v.Price).Take(5).ToList();
            return Ok(recommendedVehicles);
        }
        //get endpoints

        [HttpGet]
        public IActionResult GetVehicleList()
        {
            _response.Result = _db.Vehicles;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }
        //for calling vehicle list by id 
        [HttpGet("{id:int}", Name = "GetVehicleList")]
        public IActionResult GetVehicleList(int id)
        {
            if (id == 0)
            {
                _response.Result = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);

            }
            Vehicle vehicle = _db.Vehicles.FirstOrDefault(u => u.Id == id);
            //below logic is run when there is no items in the vehcile databases
            if (vehicle == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            _response.Result = vehicle;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

        //post api
        [HttpPost]
       
        [Authorize(Roles = Role.Vehicle + "," + Role.Admin)]

        public async Task<ActionResult<ApiResponse>> CreateVehicleList([FromForm] VehicleCreateDto vehicleCreateDto, IFormFile file)
            {
                //from form is used because we also upload image when creating hotel list

                try
                {
                    if (ModelState.IsValid)//modelt state will check required filed in model are valid or not 
                    {
                       
                        Vehicle vehicleListToCreate = new()
                        {
                            Name = vehicleCreateDto.Name,
                            Price = vehicleCreateDto.Price,
                            Email = vehicleCreateDto.Email,
                            Image = await _fileService.WriteFile(file),
                            //Image = vehicleCreateDto.Image,
                            PhoneNumber = vehicleCreateDto.PhoneNumber,
                            Rating = vehicleCreateDto.Rating,
                            Location = vehicleCreateDto.Location,
                            Review = vehicleCreateDto.Review,
                            LongDescription = vehicleCreateDto.LongDescription,
                            ShortDescription = vehicleCreateDto.ShortDescription,
                            Category = "VehicleBooking" // Default value for Category
                        };

                        _db.Add(vehicleListToCreate);
                      await _db.SaveChangesAsync(); // Await the SaveChangesAsync method
                    _response.Result = vehicleListToCreate;
                        _response.StatusCode = HttpStatusCode.Created;
                        return CreatedAtRoute("GetVehicleList", new { id = vehicleListToCreate.Id }, _response);

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
        //[Authorize(Roles = Role.Admin)]
        [Authorize(Roles = Role.Vehicle + "," + Role.Admin)]
        public async Task<ActionResult<ApiResponse>> UpdateVehicleList(int id, [FromForm] VehicleUpdateDto vehicleUpdateDto, IFormFile file)
        {
            //from form is used because we also upload image when creating hotel list

            try
            {
                if (ModelState.IsValid)//modelt state will check required filed in model are valid or not 
                {

                    if (vehicleUpdateDto == null || id != vehicleUpdateDto.Id)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }

                    //vehicle vehicleistFromDb = await _db.Hotels.FirstOrDefaultAsync(h => h.Id == id);  or below
                    Vehicle vehicleListFromDb = await _db.Vehicles.FindAsync(id);

                    if (vehicleListFromDb == null)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    vehicleListFromDb.Name = vehicleUpdateDto.Name;
                    vehicleListFromDb.Review = vehicleUpdateDto.Review;
                    vehicleListFromDb.PhoneNumber = vehicleUpdateDto.PhoneNumber;
                    vehicleListFromDb.Rating = vehicleUpdateDto.Rating;
                    vehicleListFromDb.Email = vehicleUpdateDto.Email;
                    vehicleListFromDb.ShortDescription = vehicleUpdateDto.ShortDescription;
                    vehicleListFromDb.LongDescription = vehicleUpdateDto.LongDescription;
                    vehicleListFromDb.Location = vehicleUpdateDto.Location;
                    vehicleListFromDb.Price = vehicleUpdateDto.Price;
                    //vehicleListFromDb.Image = vehicleUpdateDto.Image;
                    vehicleListFromDb.Image = await _fileService.WriteFile(file);

                    //if i upload image from file  i need to check image condition


                    _db.Update(vehicleListFromDb);
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
        public async Task<ActionResult<ApiResponse>> DeleteVehicleList(int id)
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
                Vehicle vehicleListFromDb = await _db.Vehicles.FindAsync(id);

                if (vehicleListFromDb == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                }


                // for adding millisecond delay to the code when we are calling it 
                int millisecond = 2000;
                Thread.Sleep(millisecond);

                _db.Remove(vehicleListFromDb);
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


