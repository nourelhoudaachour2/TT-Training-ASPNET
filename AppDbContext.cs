using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NuGet.DependencyResolver;

namespace Training_tunisie_telecome
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Formation> Formations { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
    }
}