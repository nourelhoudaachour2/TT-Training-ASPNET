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
        // DBSets
        // =======================

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Trainer> Trainers { get; set; }

        public DbSet<Formation> Formations { get; set; }

        public DbSet<Domain> Domains { get; set; }

        public DbSet<FormationDomain> FormationDomains { get; set; }

        public DbSet<SessionFormation> SessionFormations { get; set; }

        public DbSet<SessionParticipant> SessionParticipants { get; set; }

        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Responsable> Responsables { get; set; }


        // =======================
        // MODEL CONFIGURATION
        // =======================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =======================
            // Responsable
            // =======================

            modelBuilder.Entity<Responsable>()
                .HasIndex(r => r.Matricule)
                .IsUnique();



            // =======================
            // Employee
            // =======================

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Matricule)
                .IsUnique();


            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Domain)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DomainId)
                .OnDelete(DeleteBehavior.Restrict);



            // =======================
            // Formation - Domain
            // Many To Many
            // =======================

            modelBuilder.Entity<FormationDomain>()
                .HasOne(fd => fd.Formation)
                .WithMany(f => f.FormationDomains)
                .HasForeignKey(fd => fd.FormationId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<FormationDomain>()
              
    .HasOne(fd => fd.Domain)
    .WithMany(d => d.FormationDomains)
    .HasForeignKey(fd => fd.DomainId)
    .OnDelete(DeleteBehavior.Cascade);
                



            // =======================
            // Formation - Session
            // =======================

            modelBuilder.Entity<SessionFormation>()
                .HasOne(s => s.Formation)
                .WithMany(f => f.Sessions)
                .HasForeignKey(s => s.FormationId)
                .OnDelete(DeleteBehavior.Cascade);



            // =======================
            // Trainer - Session
            // =======================

            modelBuilder.Entity<SessionFormation>()
                .HasOne(s => s.Trainer)
                .WithMany(t => t.Sessions)
                .HasForeignKey(s => s.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);



            // =======================
            // Employee - SessionParticipant
            // =======================

            modelBuilder.Entity<SessionParticipant>()
                .HasOne(sp => sp.Employee)
                .WithMany(e => e.SessionParticipants)
                .HasForeignKey(sp => sp.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);



            // =======================
            // Session - Participant
            // =======================

            modelBuilder.Entity<SessionParticipant>()
                .HasOne(sp => sp.SessionFormation)
                .WithMany(s => s.Participants)
                .HasForeignKey(sp => sp.SessionFormationId)
                .OnDelete(DeleteBehavior.Cascade);



            // =======================
            // Remplaçant
            // Employee self relation
            // =======================

            modelBuilder.Entity<SessionParticipant>()
                .HasOne(sp => sp.ReplacementEmployee)
                .WithMany()
                .HasForeignKey(sp => sp.ReplacementEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);



            // =======================
            // Evaluation
            // =======================

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.SessionParticipant)
                .WithMany()
                .HasForeignKey(e => e.SessionParticipantId)
                .OnDelete(DeleteBehavior.Cascade);



            // =======================
            // Notification
            // =======================

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Employee)
                .WithMany()
                .HasForeignKey(n => n.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}