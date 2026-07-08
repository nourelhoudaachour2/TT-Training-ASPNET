using Microsoft.EntityFrameworkCore;
using Training_tunisie_telecome.Models;

namespace Training_tunisie_telecome.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // =======================
        // 🟢 DBSets
        // =======================

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Formation> Formations { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Responsable> Responsables { get; set; }

        // =======================
        // 🟢 MODEL CONFIGURATION
        // =======================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Responsable>()
                .HasIndex(r => r.Matricule)
                .IsUnique();
            // =======================
            // 👤 Employee
            // =======================

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Matricule)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Service)
                .WithMany(s => s.Employees)
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // =======================
            // 🏢 Service
            // =======================

            modelBuilder.Entity<Service>()
                .HasMany(s => s.Employees)
                .WithOne(e => e.Service)
                .HasForeignKey(e => e.ServiceId);

            // =======================
            // 🎓 Formation
            // =======================

            modelBuilder.Entity<Formation>()
                .HasOne(f => f.Service)
                .WithMany()
                .HasForeignKey(f => f.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Formation>()
                .HasOne(f => f.Trainer)
                .WithMany(t => t.Formations)
                .HasForeignKey(f => f.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            // =======================
            // 👨‍🏫 Trainer
            // =======================

            modelBuilder.Entity<Trainer>()
                .HasMany(t => t.Formations)
                .WithOne(f => f.Trainer)
                .HasForeignKey(f => f.TrainerId);

            // =======================
            // 📋 Attendance
            // =======================

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Formation)
                .WithMany(f => f.Attendances)
                .HasForeignKey(a => a.FormationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}