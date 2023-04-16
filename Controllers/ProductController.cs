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
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class ProductController : ControllerBase
    {
        private readonly InternetShopContext _internetShopContext;

        public ProductController(InternetShopContext context)
        {
            _internetShopContext = context;
        }

        // GET: api/<ProductController>
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
                    Price = Convert.ToString(pr.Price),
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

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> Get([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
                        Price = Convert.ToString(pr.Price),
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

        // POST api/<ProductController>
        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody] ProductDTO prDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Product pr = new()
            {
                Category = prDTO.CategoryId,
                Manufacturer = prDTO.ManufacturerId,
                Name = prDTO.Name,
                Price = Convert.ToInt32(prDTO.Price),
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

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] ProductDTO prDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var pr = await _internetShopContext.Products.FindAsync(id);
            if (pr == null)
            {
                return NotFound();
            }

            pr.Id = id;
            pr.Category = prDTO.CategoryId;
            pr.Manufacturer = prDTO.ManufacturerId;
            pr.Name = prDTO.Name;
            pr.Price = Convert.ToInt32(prDTO.Price);
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
            prDTO.Id = id;
            _internetShopContext.Products.Update(pr);
            await _internetShopContext.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
