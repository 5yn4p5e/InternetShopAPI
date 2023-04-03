using InternetShop.InternetShopModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InternetShop.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(InternetShopContext context)
        {
            try
            {
                context.Database.EnsureCreated();
                if (!context.Categories.Any())
                {
                    var categories = new Category[]
                    {
                        new Category { Name = "Смартфоны" },
                        new Category { Name = "Комплектующие для ПК" },
                        new Category { Name = "Ноутбуки" }
                    };
                    foreach (Category c in categories)
                    {
                        context.Categories.Add(c);
                    }
                    await context.SaveChangesAsync();
                }
                if (!context.Manufacturers.Any())
                {
                    var manufacturers = new Manufacturer[]
                    {
                        new Manufacturer { Name = "MSI", Address = "China, Beijing"},
                        new Manufacturer { Name = "Intel", Address = "USA, Silicon Valley"}
                    };
                    foreach (Manufacturer m in manufacturers)
                    {
                        context.Manufacturers.Add(m);
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}