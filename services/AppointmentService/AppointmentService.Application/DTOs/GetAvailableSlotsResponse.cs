using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.DTOs
{
    public class GetAvailableSlotsResponse
    {
        public List<DateTime> AvailableSlots { get; set; }
    }
}
