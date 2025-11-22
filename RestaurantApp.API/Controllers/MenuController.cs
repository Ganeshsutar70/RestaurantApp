using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.API.Data;
using RestaurantApp.API.Models;
using RestaurantApp.API.Repositories;
using RestaurantApp.API.DTOs;

namespace RestaurantApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    
    public class MenuController : ControllerBase
    {
        public readonly IRepository<MenuItem> _repository;
        public readonly RestaurantContext _context;

        public MenuController(IRepository<MenuItem> repository, RestaurantContext context)
        {
            _repository = repository;
            _context = context;
        }
        [HttpPost("AddMenu")]
        public async Task<IActionResult> AddMenu([FromForm] MenuItemDto dto)
        {

             if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            if (!dto.Price.HasValue)
                return BadRequest("Price is required.");

            byte[]? imageBytes = null;
            string? contentType = null;
            string? fileName = null;

            if (dto.Image != null)
            {
                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
                contentType = dto.Image.ContentType;
                fileName = dto.Image.FileName;
            }

            var menuItem = new MenuItem
            {
                Name = dto.Name!,
                Description = dto.Description,
                Price = dto.Price!.Value,
                Image = imageBytes,
                ImageContentType = contentType,
                ImageFileName = fileName
            };

            // _context.MenuItems.Add(menuItem);
            // await _context.SaveChangesAsync();
            await _repository.AddAsync(menuItem);

            return Ok(new { message = "Menu item added successfully!", menuItem.Id });
       
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repository.GetAllAsync();
            return Ok(items);   
        }
         

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MenuItem menuItem)
        {
            await _repository.AddAsync(menuItem);
            return CreatedAtAction(nameof(GetById), new { id = menuItem.Id }, menuItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] MenuItem updatedItem)
        {
            await _repository.UpdateAsync(updatedItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
