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
    /// <summary>
    /// Контроллер для взаимодействия с категориями
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class CategoryController : ControllerBase
    {
        private readonly InternetShopContext _internetShopContext;

        /// <summary>
        /// Подключает контроллер к контексту базы данных
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public CategoryController(InternetShopContext context)
        {
            _internetShopContext = context;
        }

        /// <summary>
        /// Получение всех категорий из базы данных
        /// </summary>
        /// <returns>Список категорий</returns>
        [HttpGet]
        public async Task<ActionResult<List<Category>>> Get()
        {
            return await _internetShopContext.Categories.ToListAsync();
        }

        /// <summary>
        /// Получения конкретной категории из базы данных
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns>Категория с данным id</returns>
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

        /// <summary>
        /// Создание новой категории в базе данных (только администратор)
        /// </summary>
        /// <param name="category">Новая категория (без id)</param>
        /// <returns>Новая категория с заполненным id</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Category>> Post([FromBody] Category category)
        {
            _internetShopContext.Categories.Add(category);
            await _internetShopContext.SaveChangesAsync();
            return CreatedAtAction("Get", new { id = category.Id }, category);
        }

        /// <summary>
        /// Изменение категории в базе данных (только администратор)
        /// </summary>
        /// <param name="categ">Категория с параметром id</param>
        /// <returns></returns>
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

        /// <summary>
        /// Удаление категории из базы данных, включая
        /// все связанные с ней товары (только администратор)
        /// </summary>
        /// <param name="id">id удаления</param>
        /// <returns></returns>
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
