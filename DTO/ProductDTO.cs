namespace InternetShop.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? CategoryName { get; set; }

        public int CategoryId { get; set; }

        public string? ManufacturerName { get; set; }

        public int ManufacturerId { get; set; }

        public int Price { get; set; }

        public string? Image { get; set; }
    }
}
