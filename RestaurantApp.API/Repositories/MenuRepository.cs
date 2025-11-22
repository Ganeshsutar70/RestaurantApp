using Microsoft.EntityFrameworkCore;
using RestaurantApp.API.Data;
using RestaurantApp.API.Models;
namespace RestaurantApp.API.Repositories
{
    public class MenuRepository : IRepository<MenuItem>
    {
        private readonly RestaurantContext _context;

        public MenuRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems.ToListAsync();
        }

        public async Task<MenuItem> GetByIdAsync(int id)
        {
            return await _context.MenuItems.FindAsync(id);
        }

        public async Task AddAsync(MenuItem entity)
        {
            await _context.MenuItems.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MenuItem entity)
        {
            var existingItem = await _context.MenuItems.FindAsync(entity.Id);
            if (existingItem == null)
            {
                throw new KeyNotFoundException("Menu item not found.");
            }
            // update the tracked entity's values with the incoming entity's values
            _context.Entry(existingItem).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}