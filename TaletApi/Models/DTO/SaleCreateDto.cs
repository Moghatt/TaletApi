namespace TaletApi.Models.DTO
{
    public class SaleCreateDto
    {
        public DateTime DateSold { get; set; }
        public string CustomerName { get; set;}
        public string ProductName { get; set; }
        public string StoreName { get; set; }
    }
}
