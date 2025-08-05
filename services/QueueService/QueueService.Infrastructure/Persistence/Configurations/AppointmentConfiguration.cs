using QueueService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QueueService.Infrastructure.Persistence.Configurations
{
    //public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    //{
    //    public void Configure(EntityTypeBuilder<Appointment> builder)
    //    {
    //        builder.ToTable("Appointments");
    //        builder.HasKey(a => a.Id);

    //        builder.Property(a => a.DoctorId).IsRequired();
    //        builder.Property(a => a.PatientId).IsRequired();
    //        builder.Property(a => a.AppointmentTime).IsRequired();
    //        builder.Property(a => a.QueueGenerated).HasDefaultValue(false);

    //        // ✅ Indexes
    //        builder.HasIndex(a => a.AppointmentTime);
    //        builder.HasIndex(a => a.QueueGenerated);
    //        builder.HasIndex(a => new { a.DoctorId, a.PatientId });
    //        builder.HasIndex(a => new { a.DoctorId, a.AppointmentTime });
    //    }
    //}
}
