//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using PatientService.Domain.Entities;

//namespace PatientService.Infrastructure.Persistence.Configurations
//{
//    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
//    {
//        public void Configure(EntityTypeBuilder<Patient> builder)
//        {
//            builder.ToTable("Patients");

//            builder.HasKey(q => q.Id);

//            builder.Property(q => q.PatientId).IsRequired();
//            builder.Property(q => q.DoctorId).IsRequired();
//            builder.Property(q => q.AppointmentId).IsRequired();
//            builder.Property(q => q.DepartmentId).IsRequired().HasMaxLength(50);
//            builder.Property(q => q.HospitalId).IsRequired();

//            builder.Property(q => q.Status).HasConversion<int>().IsRequired();
//            builder.Property(q => q.Position).IsRequired();
//            builder.Property(q => q.EstimatedWaitTime).IsRequired();
//            builder.Property(q => q.QueueNumber).HasMaxLength(20).IsRequired();
//            builder.Property(q => q.JoinedAt).IsRequired();
//            builder.Property(q => q.CalledAt).IsRequired(false);
//            builder.Property(q => q.CompletedAt).IsRequired(false);
//            builder.Property(q => q.SkippedAt).IsRequired(false);
//            builder.Property(q => q.CancelledAt).IsRequired(false);

//            // Indexes for performance
//            builder.HasIndex(q => new { q.DepartmentId, q.Status, q.Position })
//                   .HasDatabaseName("IX_QueueItems_Department_Status_Position");

//            builder.HasIndex(q => new { q.PatientId, q.Status })
//                   .HasDatabaseName("IX_QueueItems_PatientId_Status");

//            builder.HasIndex(q => new { q.DoctorId, q.Status, q.Position })
//                   .HasDatabaseName("IX_QueueItems_DoctorId_Status_Position");

//            builder.HasIndex(q => q.SkippedAt)
//                   .HasDatabaseName("IX_QueueItems_SkippedAt");

//            builder.HasIndex(q => q.CancelledAt)
//                   .HasDatabaseName("IX_QueueItems_CancelledAt");

//            // Ignore domain events and value objects
//            builder.Ignore(q => q.DomainEvents);
//            builder.Ignore(q => q.PatientInfo);
//            builder.Ignore(q => q.DoctorInfo);
//        }
//    }
//}
