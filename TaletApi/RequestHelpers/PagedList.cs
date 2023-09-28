using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace TaletApi.RequestHelpers
{
    public class PagedList<Object> : List<Object>
    {
        public MetaData MetaData { get; set; }
        public PagedList(List<Object> items, int count,int pageNumber, int pageSize) 
        {
            MetaData = new MetaData
            {
                TotalCount = count,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
            AddRange(items);

            
        }
         public static async Task<PagedList<Object>> ToPagedList(IQueryable<Object> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<Object>(items, count, pageNumber, pageSize);

        }
      
    }
}
