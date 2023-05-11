using InternetShop.DTO;
using InternetShop.InternetShopModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InternetShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class CategoryController : ControllerBase
    {
        private readonly InternetShopContext _internetShopContext;

        public CategoryController(InternetShopContext context)
        {
            _internetShopContext = context;
        }

        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<ActionResult<List<Category>>> Get()
        {
            return await _internetShopContext.Categories.ToListAsync();
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var category = await _internetShopContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // POST api/<CategoryController>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Category>> Post([FromBody] Category category)
        {
            _internetShopContext.Categories.Add(category);
            await _internetShopContext.SaveChangesAsync();
            return CreatedAtAction("Get", new { id = category.Id }, category);
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Category>>> Put([FromBody] Category categ)
        {
            var c = await _internetShopContext.Categories.FindAsync(categ.Id);
            if (c == null)
            {
                return NotFound();
            }
            c.Name = categ.Name;
            _internetShopContext.Categories.Update(c);
            await _internetShopContext.SaveChangesAsync();
            return await Get();
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var categ = await _internetShopContext.Categories.FindAsync(id);
            if (categ == null)
            {
                return NotFound();
            }
            await foreach (var pr in _internetShopContext.Products)
            {
                if (pr.Category == id)
                {
                    _internetShopContext.Products.Remove(pr);
                }
            }
            _internetShopContext.Categories.Remove(categ);
            await _internetShopContext.SaveChangesAsync();
            return Ok("Сущность успешно удалена");
        }
    }
}
