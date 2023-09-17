namespace InternetShop.InternetShopModels;

public partial class ProductOrd
{
    public int Id { get; set; }

    public int Product { get; set; }

    public int Ord { get; set; }

    public int Number { get; set; }

    public bool Ready { get; set; }

    public virtual Ord OrdNavigation { get; set; } = null!;

    public virtual Product ProductNavigation { get; set; } = null!;
}
