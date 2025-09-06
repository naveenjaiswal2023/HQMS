using AppointmentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.DTOs
{
    public record AppointmentSummaryDto(
        Guid Id,
        string Title,
        DateTime StartDateTime,
        DateTime EndDateTime,
        AppointmentStatus Status,
        AppointmentType Type,
        string PatientName,
        string DoctorName,
        string HospitalName,
        string Location
    );
}
