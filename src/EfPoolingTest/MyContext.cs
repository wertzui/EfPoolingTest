using Microsoft.EntityFrameworkCore;

namespace EfCoreLongStringTest
{
    public class MyContext : DbContext
    {
        public DbSet<MyTable> MyTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Initial Catalog=EfCorePoolingTest;Integrated Security=true;",
                    options => options.EnableRetryOnFailure());
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}