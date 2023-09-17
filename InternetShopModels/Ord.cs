namespace InternetShop.InternetShopModels;

public partial class Ord
{
    public int Id { get; set; }

    public int Number { get; set; }

    public DateTime Date { get; set; }

    public int Client { get; set; }

    public int Price { get; set; }

    public virtual ICollection<ProductOrd> ProductOrds { get; } = new List<ProductOrd>();
}
