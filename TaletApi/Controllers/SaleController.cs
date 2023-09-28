using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaletApi.Models;
using TaletApi.Models.DTO;
using System.Net;
using TaletApi.Extentions;
using TaletApi.RequestHelpers;
using System.Drawing.Printing;

namespace TaletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly SalesDbContext _db;
        private readonly ILogger<SaleController> _logger;

      


        public SaleController(SalesDbContext db, ILogger<SaleController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: api/Sale
        [HttpGet]
        public async Task<IActionResult> GetAllSales([FromQuery] PaginationParams paginationParams)
        {
          if (_db.Sales == null)
          {
              return NotFound();

          }
            var query =  _db.Sales
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .Include(r => r.Store)
                .Select(x => new { 
                    Customer = x.Customer.Name, 
                    Product = x.Product.Name, 
                    Store = x.Store.Name, 
                    DateSold =x.DateSold.ToShortDateString(),
                    Id=x.Id
                }).AsQueryable();

            var sale = await PagedList<Object>.ToPagedList(query, paginationParams.PageNumber, paginationParams.PageSize);
            Response.AddPaginationHeader(sale.MetaData);

            return Ok(sale);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleById(int id)
        {

            var sale = await _db.Sales.FindAsync(id);

            if (sale == null)
            {
                return NotFound();
            }

            return Ok(sale);
        }


        // PUT: api/Sale/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
     
        public async Task<IActionResult> UpdateSale(int id, SaleUpdateDTO saleUpdateDTO)
        {
            var sale = await _db.Sales
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .Include(r => r.Store)
                .FirstOrDefaultAsync(x => x.Id == id);

            Console.WriteLine(sale.ToString());
            Console.WriteLine(sale.Customer);
            if (sale == null || sale.Id != id)
            {
                return BadRequest();
            }

            // Attempt to find the associated customer by the old name
            Customer customer = await _db.Customers.FirstOrDefaultAsync(c => c.Name == sale.Customer.Name);

            // Attempt to find the associated product by the old name
            Product product = await _db.Products.FirstOrDefaultAsync(p => p.Name == sale.Product.Name);

            // Attempt to find the associated store by the old name
            Store store = await _db.Stores.FirstOrDefaultAsync(s => s.Name == sale.Store.Name);
            

            if (customer != null && product != null && store != null)
            {
                // If all associated entities are found, update their names
                customer.Name = saleUpdateDTO.CustomerName;
                product.Name = saleUpdateDTO.ProductName;
                store.Name = saleUpdateDTO.StoreName;

                sale.DateSold = saleUpdateDTO.DateSold;

                await _db.SaveChangesAsync();

                // Return a success response
                return Ok();
            }
            else
            {
                // Handle the case where one or more associated entities were not found
                return BadRequest("One or more of the specified entities (customer, product, store) do not exist.");
            }



        }

        // POST: api/Sale
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddSale(SaleCreateDto saleCreateDto)
        {
          if (_db.Sales == null)
          {
                return NotFound();
          }

            try
            {
                if (ModelState.IsValid)
                {

                    Customer customer = await _db.Customers.FirstOrDefaultAsync(c => c.Name == saleCreateDto.CustomerName);
                    Product product = await _db.Products.FirstOrDefaultAsync(p => p.Name == saleCreateDto.ProductName);
                    Store store = await _db.Stores.FirstOrDefaultAsync(s => s.Name == saleCreateDto.StoreName);

                    if (customer != null && product != null && store != null)
                    {
                        Sale newSale = new ()
                        {
                            Customer = customer,
                            Product = product,
                            Store = store,
                            DateSold = saleCreateDto.DateSold
                        };
                  

                        await _db.Sales.AddAsync(newSale);
                        await _db.SaveChangesAsync();
                        var jsonSerializerOptions = new JsonSerializerOptions
                        {
                            ReferenceHandler = ReferenceHandler.Preserve,
                            WriteIndented = true // Optional for readability
                        };

                        //return new JsonResult(newSale, jsonSerializerOptions);


                        return StatusCode((int)HttpStatusCode.Created);
                    }
                    else
                    {
                        return BadRequest("One of the Input is not Exsited");
                    }
                  

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
                return BadRequest(ex.Message);
                
                //return StatusCode(500, "Internal Server Error"); // Return a generic error response
            }
        }

        // DELETE: api/Sale/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            if (_db.Sales == null)
            {
                return NotFound();
            }
            var sale = await _db.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            _db.Sales.Remove(sale);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool SaleExists(int id)
        {
            return (_db.Sales?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
