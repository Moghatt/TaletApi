
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using TaletApi.Extentions;
using TaletApi.Models;
using TaletApi.Models.DTO;
using TaletApi.RequestHelpers;

namespace TaletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly SalesDbContext _db;
   

        public CustomerController(SalesDbContext db)
        {
            _db = db;
               
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers([FromQuery] PaginationParams paginationParams)
        {


                if (_db.Customers == null)
            {
                return NotFound();
            }
                

            var query = _db.Customers.AsQueryable();
            /*
            if (paginationParams.OrderBy == "des")
            {
                query = query.OrderByDescending(p => p.Name);
            }
            else
            {
                query = query.OrderBy(p => p.Name);
            }
            */

            var customers = await PagedList<Customer>.ToPagedList(query, paginationParams.PageNumber, paginationParams.PageSize);
            Response.AddPaginationHeader(customers.MetaData);

            return  Ok(customers);

               
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {

            var customer =  _db.Customers
    .Include(c => c.Sales) // Include the associated sales and products
    .ToList()
    .Select(c => new
    {
        id = c.Id,
        name = c.Name,
        sales = c.Sales.Select(s => new
        {
            saleId = s.Id,
            saleDate = s.DateSold,
      
            productName = s.Product?.Name, productPrice = s.Product?.Price,
        })
    })
    .ToList();


            if (customer == null)
            {
                return NotFound();


            }
            return Ok(customer);

            
            //var saleRecord = _db.Sales.Where(e => e.CustomerId == customer.Id).Select(x =>new
            //{
            //    x.CustomerId,
            //    productName = x.Product.Name,
            //    productPrice = x.Product.Price,
            //    storeName = x.Store.Name,
            //    x.DateSold,

            //});    

            //return Ok(saleRecord);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> AddCustomer(CustomerCreateDto customerCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Customer newCustomer = new()
                    {
                        Name = customerCreateDTO.Name,
                        Address = customerCreateDTO.Address,
                    };

                    await _db.Customers.AddAsync(newCustomer);
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

                return StatusCode(500, ex.Message); // Return a generic error response
            }
        }
       

                [HttpPut("{id}")]
                public async Task<IActionResult> UpdateCustomer(int id, CustomerUpdateDTO customerUpdateDTO)
                {


                    var customer = await _db.Customers.FindAsync(id);
                    if (customer == null || customer.Id!= id)
                    {
                        return BadRequest();
                    }

                    Customer updatedCustomer = new()
                    {
                       
                        Name = customerUpdateDTO.Name,
                        Address = customerUpdateDTO.Address,
                    };

                    customer.Name = customerUpdateDTO.Name;
                    customer.Address = customerUpdateDTO.Address;
            await _db.SaveChangesAsync();
                    
                    return Ok(updatedCustomer);
                }
       
               [HttpDelete("{id}")]
               public async Task<IActionResult> DeleteCustomer(int id)
               {
            try
            {
                var customer = await _db.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                _db.Customers.Remove(customer);
                await _db.SaveChangesAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

                   

               }
       
    }
}
