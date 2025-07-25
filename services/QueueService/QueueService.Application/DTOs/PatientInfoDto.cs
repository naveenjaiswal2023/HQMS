using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.DTOs
{
    public class PatientInfoDto
    {
        public Guid Id { get; set; }
        public string UHID { get; set; }  // Unique Hospital ID
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
