using BookingManagement.MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.MVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<ConsultantSchedule> ConsultantSchedules { get; set; }
        public DbSet<ConsultantBreak> ConsultantBreaks { get; set; }
        public DbSet<AppointmentBooking> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(customer => customer.Email).IsUnique();
                entity.Property(customer => customer.AddedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(customer => customer.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(customer => customer.Status).HasDefaultValue(true);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(user => user.Email).IsUnique();
                entity.Property(user => user.Role).HasDefaultValue(1);
                entity.Property(user => user.Status).HasDefaultValue(true);
            });

            modelBuilder.Entity<Consultant>(entity =>
            {
                entity.HasIndex(consultant => consultant.Email).IsUnique();
                entity.HasIndex(consultant => new { consultant.Status, consultant.Specialization });
                entity.Property(consultant => consultant.AddedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(consultant => consultant.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(consultant => consultant.Status).HasDefaultValue(true);
            });

            modelBuilder.Entity<ConsultantSchedule>(entity =>
            {
                entity.HasOne(schedule => schedule.Consultant)
                    .WithMany(consultant => consultant.WorkingSchedules)
                    .HasForeignKey(schedule => schedule.ConsultantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(schedule => new { schedule.ConsultantId, schedule.Day }).IsUnique();
                entity.Property(schedule => schedule.IsWorking).HasDefaultValue(true);
            });

            modelBuilder.Entity<ConsultantBreak>(entity =>
            {
                entity.HasOne(consultantBreak => consultantBreak.ConsultantSchedule)
                    .WithMany(schedule => schedule.Breaks)
                    .HasForeignKey(consultantBreak => consultantBreak.ConsultantScheduleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AppointmentBooking>(entity =>
            {
                entity.HasOne(appointment => appointment.Consultant)
                    .WithMany(consultant => consultant.Appointments)
                    .HasForeignKey(appointment => appointment.ConsultantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(appointment => appointment.Customer)
                    .WithMany(customer => customer.Appointments)
                    .HasForeignKey(appointment => appointment.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(appointment =>
                    new { appointment.ConsultantId, appointment.AppointmentDate, appointment.TimeSlot })
                    .IsUnique();

                entity.Property(appointment => appointment.CreatedAt)
                    .HasDefaultValueSql("SYSUTCDATETIME()");

                entity.Property(appointment => appointment.Status).HasDefaultValue(1);
            });
        }
    }
}
