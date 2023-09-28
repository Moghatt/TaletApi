using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaletApi.Models.DTO;
using TaletApi.Models;
using TaletApi.RequestHelpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using TaletApi.Extentions;

namespace TaletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly SalesDbContext _db;


        public ProductController(SalesDbContext db)
        {
            _db = db;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] PaginationParams paginationParams)
        {
            if (_db.Products == null)
            {
                return NotFound();

            }
            var query = _db.Products.AsQueryable();
            var products = await PagedList<Product>.ToPagedList(query, paginationParams.PageNumber, paginationParams.PageSize);
            Response.AddPaginationHeader(products.MetaData);

            return Ok(products);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {

            var product = await _db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct(ProductCreateDto productCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Product newProduct = new()
                    {
                        Name = productCreateDTO.Name,
                        Price = productCreateDTO.Price,
                    };

                    await _db.Products.AddAsync(newProduct);
                    await _db.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
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
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDTO productUpdateDTO)
        {


            var product = await _db.Products.FindAsync(id);
            if (product == null || product.Id != id)
            {
                return BadRequest();
            }

            Product updatedProduct = new()
            {

                Name = productUpdateDTO.Name,
                Price = productUpdateDTO.Price,
            };

            product.Name = productUpdateDTO.Name;
            product.Price = productUpdateDTO.Price;
            await _db.SaveChangesAsync();

            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {

            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return NoContent();

        }

    }
}

