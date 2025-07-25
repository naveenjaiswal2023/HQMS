using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.DTOs
{
    public class DoctorInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string RegistrationNumber { get; set; }
        public string Specialization { get; set; }
        public string Department { get; set; }
        public string ContactNumber { get; set; }
    }
}
