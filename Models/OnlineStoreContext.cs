using System.Data.Entity;

namespace WebApplication8.Models
{
    public class OnlineStoreContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}