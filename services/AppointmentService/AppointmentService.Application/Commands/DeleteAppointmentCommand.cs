using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands
{
    public class DeleteAppointmentCommand :  IRequest<bool>
    {
        public DeleteAppointmentCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
