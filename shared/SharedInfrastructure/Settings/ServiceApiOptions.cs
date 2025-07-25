using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInfrastructure.Settings
{
    public class ServiceApiOptions
    {
        public string HospitalApi { get; set; }
        public string AppointmentApi { get; set; }
        public string DoctorApi { get; set; }
        public string PatientApi { get; set; }
    }
}
