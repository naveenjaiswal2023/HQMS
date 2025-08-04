using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands
{
    public class UpdateAppointmentStatusCommand : IRequest<bool>
    {
        public Guid AppointmentId { get; set; }
        public bool Status { get; set; }

        public UpdateAppointmentStatusCommand(Guid appointmentId, bool status)
        {
            AppointmentId = appointmentId;
            Status = status;
        }
    }
}
