using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands
{
    public class CompleteAppointmentCommand : IRequest<bool>
    {
        public CompleteAppointmentCommand(Guid id, string notes)
        {
            Id = id;
            Notes = notes;
        }

        public Guid Id { get; }
        public string Notes { get; }
    }
}
