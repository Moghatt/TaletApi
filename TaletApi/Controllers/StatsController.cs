using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaletApi.Extentions;
using TaletApi.Models;
using TaletApi.RequestHelpers;

namespace TaletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly SalesDbContext _db;
        private readonly ILogger<StatsController> _logger;




        public StatsController(SalesDbContext db, ILogger<StatsController> logger)
        {
            _db = db;
            _logger = logger;
        }
        // GET: api/Stats
        [HttpGet("pieTotalStats")]
        public async Task<IActionResult> GetAllStats()
        {
            var totalSales = _db.Sales.Count();
            var totalCustomer = _db.Customers.Count();
            var totalStore = _db.Stores.Count();
            var totalProduct = _db.Products.Count();
           

            return Ok(new
            {
                totalSales,
                totalCustomer,
                totalStore,
                totalProduct
            });

        }

        [HttpGet("barStoresStats")]
        public async Task<IActionResult> GetAllStoresStat()
        {

            var query = _db.Sales              
                .Include(r => r.Product)
                .Include(r => r.Store)
                .Select(x => new
                {
                
                    Product = x.Product.Name,
                    Store = x.Store.Name,
                    DateSold = x.DateSold.ToShortDateString(),
                    Id = x.Id
                }).ToList();
         


            return Ok(query);

        }
    }
}
