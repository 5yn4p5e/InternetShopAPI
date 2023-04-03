using System;
using System.Collections.Generic;

namespace InternetShop.InternetShopModels;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Category { get; set; }

    public int Manufacturer { get; set; }

    public int Price { get; set; }

    public string? Image { get; set; }

    public virtual Category CategoryNavigation { get; set; } = null!;

    public virtual Manufacturer ManufacturerNavigation { get; set; } = null!;

    public virtual ICollection<ProductOrd> ProductOrds { get; } = new List<ProductOrd>();
}
