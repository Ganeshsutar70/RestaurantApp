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
        [HttpPost("addmenu")]
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
                ImageFileName = fileName,
                FoodType = dto.FoodType,
                Category = dto.Category
            };

            // _context.MenuItems.Add(menuItem);
            // await _context.SaveChangesAsync();
            await _repository.AddAsync(menuItem);

            return Ok(new { message = "Menu item added successfully!", menuItem.Id });
       
        }

        [HttpPut("updatemenu/{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromForm] MenuItemDto dto)
        {
            var existingItem = await _repository.GetByIdAsync(id);
            if (existingItem == null)
                return NotFound("Menu item not found.");    
            existingItem.Name = dto.Name ?? existingItem.Name;
            existingItem.Description = dto.Description ?? existingItem.Description;
            existingItem.FoodType = dto.FoodType ?? existingItem.FoodType;
            existingItem.Category = dto.Category ?? existingItem.Category;
            if (dto.Price.HasValue)
                existingItem.Price = dto.Price.Value;
            if (dto.Image != null)
            {
                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);
                existingItem.Image = ms.ToArray();
                existingItem.ImageContentType = dto.Image.ContentType;
                existingItem.ImageFileName = dto.Image.FileName;
            }
            await _repository.UpdateAsync(existingItem);
            return Ok(new { message = "Menu item updated successfully!" });
        }

        [HttpGet("getmenus")]
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
        [HttpGet("foodtype/{type}")]
        public async Task<IActionResult> GetMenusByType(string type)
        {
            var item =  _context.MenuItems.Where(x => x.FoodType.ToLower() == type.ToLower());
            if (item == null) return NotFound();
            return Ok(item);            
        }
        [HttpGet("category/{categoryName}")]
        public async Task<IActionResult> GetMenusByCategory(string categoryName)
        {
            var item =  _context.MenuItems.Where(x => x.Category.ToLower() == categoryName.ToLower());
            if (item == null) return NotFound();
            return Ok(item);            
        }
    }
}
