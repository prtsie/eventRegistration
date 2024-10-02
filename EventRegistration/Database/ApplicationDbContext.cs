using EventRegistration.Database.Models;
using Microsoft.EntityFrameworkCore;
using EventRegistration.Database.Models.Users;
using EventRegistration.Database.Models.Events;

namespace EventRegistration.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // DbSet для сущностей
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация сущности User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Login).IsRequired().HasMaxLength(Constraints.MaxLoginLength);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(Constraints.MaxPasswordLength);
                entity.Property(u => u.Role).IsRequired();
            });

            // Конфигурация сущности Event
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(Constraints.MaxEventNameLength);
                entity.Property(e => e.Date).IsRequired();
                entity.HasOne(e => e.Host)
                      .WithMany()
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация сущности Registration
            modelBuilder.Entity<Registration>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.HasOne(r => r.User)
                      .WithMany()
                      .IsRequired();

                entity.HasOne(r => r.Event)
                      .WithMany()
                      .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
