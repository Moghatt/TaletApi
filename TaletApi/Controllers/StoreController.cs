using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaletApi.Models.DTO;
using TaletApi.Models;
using TaletApi.RequestHelpers;
using TaletApi.Extentions;
using System.Net;

namespace TaletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly SalesDbContext _db;


        public StoreController(SalesDbContext db)
        {
            _db = db;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllStores([FromQuery] PaginationParams paginationParams)
        {
            if (_db.Stores == null)
            {
                return NotFound();
            }
            var query = _db.Stores.AsQueryable();
            var store = await PagedList<Store>.ToPagedList(query, paginationParams.PageNumber, paginationParams.PageSize);
            Response.AddPaginationHeader(store.MetaData);

            return Ok(store);


           
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreById(int id)
        {

            var store = await _db.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpPost]
        public async Task<ActionResult<Store>> AddStore(StoreCreateDto storeCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Store newStore = new()
                    {
                        Name = storeCreateDTO.Name,
                        Address = storeCreateDTO.Address,
                    };

                    await _db.Stores.AddAsync(newStore);
                    await _db.SaveChangesAsync();

                    return StatusCode((int)HttpStatusCode.Created);
                }
                else
                {
                    return BadRequest(ModelState); // Return validation errors if ModelState is not valid
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // You may also want to handle specific exceptions and return appropriate responses

                return StatusCode(500, "Internal Server Error"); // Return a generic error response
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, StoreUpdateDTO storeUpdateDTO)
        {


            var store = await _db.Stores.FindAsync(id);
            if (store == null || store.Id != id)
            {
                return BadRequest();
            }

            Store updatedStore = new()
            {

                Name = storeUpdateDTO.Name,
                Address = storeUpdateDTO.Address,
            };

            store.Name = storeUpdateDTO.Name;
            store.Address = storeUpdateDTO.Address;
            await _db.SaveChangesAsync();

            return Ok(updatedStore);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {

            var store = await _db.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            _db.Stores.Remove(store);
            await _db.SaveChangesAsync();
            return NoContent();

        }

    }
}

