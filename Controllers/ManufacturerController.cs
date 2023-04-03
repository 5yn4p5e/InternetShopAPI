using InternetShop.InternetShopModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InternetShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class ManufacturerController : ControllerBase
    {
        private readonly InternetShopContext _internetShopContext;

        public ManufacturerController(InternetShopContext context)
        {
            _internetShopContext = context;
        }

        // GET: api/<ManufacturerController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Manufacturer>>> Get()
        {
            return await _internetShopContext.Manufacturers.ToListAsync();
        }

        // GET api/<ManufacturerController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Manufacturer>> Get(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var manufacturer = await _internetShopContext.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return NotFound();
            }
            return Ok(manufacturer);
        }

        // POST api/<ManufacturerController>
        [HttpPost]
        public async Task<ActionResult<Manufacturer>> Post([FromBody] Manufacturer manuf)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _internetShopContext.Manufacturers.Add(manuf);
            await _internetShopContext.SaveChangesAsync();
            return CreatedAtAction("Get", new { id = manuf.Id }, manuf);
        }

        // PUT api/<ManufacturerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ManufacturerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
