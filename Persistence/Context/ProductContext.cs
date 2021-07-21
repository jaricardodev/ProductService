using Microsoft.EntityFrameworkCore;
using Persistence.Context.Configurations;

namespace Persistence.Context
{
    public class FeedContext : DbContext
    {
        public FeedContext()
        {
        }

        public FeedContext(DbContextOptions<FeedContext> options)
            : base(options)
        {
        }
        
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
        
    }
}
