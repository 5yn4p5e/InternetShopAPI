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
    /// Контроллер для взаимодействия с производителями
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class ManufacturerController : ControllerBase
    {
        private readonly InternetShopContext _internetShopContext;

        /// <summary>
        /// Подключает контроллер к контексту базы данных
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public ManufacturerController(InternetShopContext context)
        {
            _internetShopContext = context;
        }

        /// <summary>
        /// Получение всех производителей из базы данных
        /// </summary>
        /// <returns>Список производителей</returns>
        [HttpGet]
        public async Task<ActionResult<List<Manufacturer>>> Get()
        {
            return await _internetShopContext.Manufacturers.ToListAsync();
        }

        /// <summary>
        /// Получения конкретного производителя из базы данных
        /// </summary>
        /// <param name="id">Идентификатор производителя</param>
        /// <returns>Производитель с данным id</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Manufacturer>> Get(int id)
        {
            var manufacturer = await _internetShopContext.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return NotFound();
            }
            return Ok(manufacturer);
        }

        /// <summary>
        /// Создание нового производителя в базе данных (только администратор)
        /// </summary>
        /// <param name="manuf">Новый производитель (без id)</param>
        /// <returns>Новый производитель с заполненным id</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Manufacturer>> Post([FromBody] Manufacturer manuf)
        {
            _internetShopContext.Manufacturers.Add(manuf);
            await _internetShopContext.SaveChangesAsync();
            return CreatedAtAction("Get", new { id = manuf.Id }, manuf);
        }

        /// <summary>
        /// Изменение производителя в базе данных (только администратор)
        /// </summary>
        /// <param name="manuf">Производитель с параметром id</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Manufacturer>>> Put([FromBody] Manufacturer manuf)
        {
            var man = await _internetShopContext.Manufacturers.FindAsync(manuf.Id);
            if (man == null)
            {
                return NotFound();
            }
            man.Name = manuf.Name;
            man.Address = manuf.Address;
            _internetShopContext.Manufacturers.Update(man);
            await _internetShopContext.SaveChangesAsync();
            return await Get();
        }

        /// <summary>
        /// Удаление производителя из базы данных, включая
        /// все связанные с ним товары (только администратор)
        /// </summary>
        /// <param name="id">id удаления</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var manuf = await _internetShopContext.Manufacturers.FindAsync(id);
            if (manuf == null)
            {
                return NotFound();
            }
            await foreach (var pr in _internetShopContext.Products)
            {
                if (pr.Manufacturer == id)
                {
                    _internetShopContext.Products.Remove(pr);
                }
            }
            _internetShopContext.Manufacturers.Remove(manuf);
            await _internetShopContext.SaveChangesAsync();
            return Ok("Сущность успешно удалена");
        }
    }
}
