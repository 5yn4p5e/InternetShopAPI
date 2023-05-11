using Microsoft.AspNetCore.Cors;
using InternetShop.InternetShopModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InternetShop.DTO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Authorization;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InternetShop.Controllers
{
    /// <summary>
    /// Контроллер для взаимодействия с товарами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class ProductController : ControllerBase
    {
        private readonly InternetShopContext _internetShopContext;


        /// <summary>
        /// Подключает контроллер к контексту базы данных
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public ProductController(InternetShopContext context)
        {
            _internetShopContext = context;
        }

        /// <summary>
        /// Получение всех товаров из базы данных
        /// </summary>
        /// <returns>Список DTO товаров</returns>
        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> Get()
        {
            List<ProductDTO> products = new();
            await foreach (var pr in _internetShopContext.Products)
            {                
                ProductDTO prDTO = new()
                {
                    Id = pr.Id,
                    CategoryId = pr.Category,
                    ManufacturerId = pr.Manufacturer,
                    Name = pr.Name,
                    Price = pr.Price,
                    Image = pr.Image
                };
                products.Add(prDTO);
            }
            foreach (var pr in products)
            {
                await foreach (var categ in _internetShopContext.Categories)
                {
                    if (categ.Id == pr.CategoryId)
                    {
                        pr.CategoryName = categ.Name;
                        break;
                    }
                }
                await foreach (var manuf in _internetShopContext.Manufacturers)
                {
                    if (manuf.Id == pr.ManufacturerId)
                    {
                        pr.ManufacturerName = manuf.Name;
                        break;
                    }
                }
            }
            return products;
        }

        /// <summary>
        /// Получения конкретного товара из базы данных
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns>DTO товара с данным id</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> Get([FromRoute] int id)
        {
            ProductDTO productDTO = new();
            bool isExist = false;
            await foreach (var pr in _internetShopContext.Products)
            {
                if (pr.Id == id)
                {
                    productDTO = new()
                    {
                        Id = pr.Id,
                        CategoryId = pr.Category,
                        ManufacturerId = pr.Manufacturer,
                        Name = pr.Name,
                        Price = pr.Price,
                        Image = pr.Image
                    };
                    isExist = true;
                    break;
                }
            }
            if (!isExist)
            {
                return NotFound();
            }
            isExist = false;
            await foreach (var categ in _internetShopContext.Categories)
            {
                if (categ.Id == productDTO.CategoryId)
                {
                    productDTO.CategoryName = categ.Name;
                    isExist = true;
                    break;
                }
            }
            if (!isExist)
            {
                return NotFound();
            }
            isExist = false;
            await foreach (var manuf in _internetShopContext.Manufacturers)
            {
                if (manuf.Id == productDTO.ManufacturerId)
                {
                    productDTO.ManufacturerName = manuf.Name;
                    isExist = true;
                    break;
                }
            }
            if (!isExist)
            {
                return NotFound();
            }
            return Ok(productDTO);
        }

        /// <summary>
        /// Создание нового товара в базе данных (только администратор)
        /// </summary>
        /// <param name="prDTO">DTO нового товара (без id, разрешается без имён категории и производителя,
        /// ссылки на изображение)</param>
        /// <returns>DTO передаваемого товара с заполненными id,
        /// categoryName, manufacturerName</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Product>> Post([FromBody] ProductDTO prDTO)
        {
            Product pr = new()
            {
                Category = prDTO.CategoryId,
                Manufacturer = prDTO.ManufacturerId,
                Name = prDTO.Name,
                Price = prDTO.Price,
                Image = prDTO.Image
            };
            bool isCorrect = false;
            await foreach (var categ in _internetShopContext.Categories)
            {
                if (categ.Id == pr.Category)
                {
                    pr.CategoryNavigation = categ;
                    prDTO.CategoryName = categ.Name;
                    isCorrect = true;
                    break;
                }
            }
            if (!isCorrect)
            {
                return BadRequest(ModelState);
            }
            isCorrect = false;
            await foreach (var manuf in _internetShopContext.Manufacturers)
            {
                if (manuf.Id == pr.Manufacturer)
                {
                    pr.ManufacturerNavigation = manuf;
                    prDTO.ManufacturerName = manuf.Name;
                    isCorrect = true;
                    break;
                }
            }
            if (!isCorrect)
            {
                return BadRequest(ModelState);
            }
            _internetShopContext.Products.Add(pr);
            await _internetShopContext.SaveChangesAsync();
            prDTO.Id = pr.Id;
            return CreatedAtAction("Get", new { id = prDTO.Id }, prDTO);
        }

        /// <summary>
        /// Изменение товара в базе данных (только администратор)
        /// </summary>
        /// <param name="prDTO">DTO товара с параметром Id. Изменятся все поля</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<ProductDTO>>> Put([FromBody] ProductDTO prDTO)
        {
            var pr = await _internetShopContext.Products.FindAsync(prDTO.Id);
            if (pr == null)
            {
                return NotFound();
            }

            pr.Category = prDTO.CategoryId;
            pr.Manufacturer = prDTO.ManufacturerId;
            pr.Name = prDTO.Name;
            pr.Price = prDTO.Price;
            pr.Image = prDTO.Image;

            bool isCorrect = false;
            await foreach (var categ in _internetShopContext.Categories)
            {
                if (categ.Id == pr.Category)
                {
                    pr.CategoryNavigation = categ;
                    prDTO.CategoryName = categ.Name;
                    isCorrect = true;
                    break;
                }
            }
            if (!isCorrect)
            {
                return BadRequest(ModelState);
            }
            isCorrect = false;
            await foreach (var manuf in _internetShopContext.Manufacturers)
            {
                if (manuf.Id == pr.Manufacturer)
                {
                    pr.ManufacturerNavigation = manuf;
                    prDTO.ManufacturerName = manuf.Name;
                    isCorrect = true;
                    break;
                }
            }
            if (!isCorrect)
            {
                return BadRequest(ModelState);
            }
            _internetShopContext.Products.Update(pr);
            await _internetShopContext.SaveChangesAsync();
            return await Get();
        }

        /// <summary>
        /// Удаление товара из базы данных (только администратор)
        /// </summary>
        /// <param name="id">id удаления</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var prod = await _internetShopContext.Products.FindAsync(id);
            if (prod == null)
            {
                return NotFound();
            }
            _internetShopContext.Products.Remove(prod);
            await _internetShopContext.SaveChangesAsync();
            return Ok("Сущность успешно удалена");
        }
    }
}
