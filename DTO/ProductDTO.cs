namespace InternetShop.DTO
{
    /// <summary>
    /// DTO товара для удобного взаимодействия между клиентской частью и серверной
    /// </summary>
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        /// <summary>
        /// Имя категории для отображения на сайте
        /// </summary>
        public string? CategoryName { get; set; }

        public int CategoryId { get; set; }

        /// <summary>
        /// Имя производителя для отображения на сайте
        /// </summary>
        public string? ManufacturerName { get; set; }

        public int ManufacturerId { get; set; }

        public int Price { get; set; }

        public string? Image { get; set; }
    }
}
