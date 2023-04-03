using System;
using System.Collections.Generic;

namespace InternetShop.InternetShopModels;

public partial class Manufacturer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
