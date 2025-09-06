using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries
{
    public record GetAppointmentWithDetailsQuery(Guid Id) : IRequest<AppointmentWithDetailsResponse>;

}
