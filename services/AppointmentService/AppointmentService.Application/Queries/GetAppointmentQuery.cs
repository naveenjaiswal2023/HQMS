using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries
{
    public class GetAppointmentQuery
    {
        public GetAppointmentQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
