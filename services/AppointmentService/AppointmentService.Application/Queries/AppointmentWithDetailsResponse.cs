//using AppointmentService.Application.DTOs;
using AppointmentService.Domain.Enums;
using SharedInfrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries
{
    public record AppointmentWithDetailsResponse(
        Guid Id,
        string Title,
        string Description,
        DateTime StartDateTime,
        DateTime EndDateTime,
        AppointmentStatus Status,
        AppointmentType Type,
        string Location,
        string? Notes,
        decimal Fee,
        bool IsPaid,
        PatientDto Patient,
        DoctorDto Doctor,
        HospitalDto Hospital,
        Guid? QueueId,
        Guid? PaymentId,
        DateTime Created,
        DateTime? LastModified
    );
}
