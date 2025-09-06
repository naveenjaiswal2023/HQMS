using AppointmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointmentService.Infrastructure.Persistence.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            // Primary Key
            builder.HasKey(a => a.Id);

            // Required fields
            builder.Property(a => a.DoctorId).IsRequired();
            builder.Property(a => a.PatientId).IsRequired();
            builder.Property(a => a.DepartmentId).IsRequired();
            builder.Property(a => a.HospitalId).IsRequired();
            builder.Property(a => a.ScheduledDate).IsRequired();
            builder.Property(a => a.ScheduledTime).IsRequired();

            // Enum properties stored as strings
            builder.Property(a => a.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.BookedAt)
                .IsRequired();

            builder.Property(a => a.Remarks)
                .HasMaxLength(500);

            // ✅ New property: IsQueueGenerated
            builder.Property(a => a.IsQueueGenerated)
                .IsRequired()
                .HasDefaultValue(false);

            // Indexes for performance
            builder.HasIndex(a => a.ScheduledDate);
            builder.HasIndex(a => new { a.DoctorId, a.PatientId });
            builder.HasIndex(a => new { a.DoctorId, a.ScheduledDate, a.ScheduledTime });
        }
    }
}
