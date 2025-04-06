using Microsoft.EntityFrameworkCore;

namespace MunicipalityManagement.Models
{
    public class MunicipalityDbContext : DbContext
    {
        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Report> Reports { get; set; }

        public MunicipalityDbContext(DbContextOptions<MunicipalityDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Citizen>().ToTable("Citizen");
            modelBuilder.Entity<ServiceRequest>().ToTable("ServiceRequest");
            modelBuilder.Entity<Staff>().ToTable("Staff");
            modelBuilder.Entity<Report>().ToTable("Reports");
        }
    }
}
