using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Events
{
    public class QueueItemCalledEvent
    {
        public string QueueItemId { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public DateTime CalledAt { get; set; }
    }
}
