using Microsoft.EntityFrameworkCore;
using RestaurantApp.API.Models;

namespace RestaurantApp.API.Data
{
    public class RestaurantContext : DbContext
    {
        public RestaurantContext(DbContextOptions<RestaurantContext> options) : base(options)
        {
        }

        public DbSet<MenuItem> MenuItems { get; set; }
    }
}
