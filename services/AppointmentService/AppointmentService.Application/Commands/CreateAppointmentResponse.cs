using AppointmentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands
{
    public record CreateAppointmentResponse(
        Guid Id,
        string Title,
        DateTime StartDateTime,
        DateTime EndDateTime,
        AppointmentStatus Status,
        decimal Fee,
        bool RequiresPayment
    );
}
