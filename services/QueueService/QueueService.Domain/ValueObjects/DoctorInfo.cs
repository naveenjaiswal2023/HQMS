using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.ValueObjects
{
    public class DoctorInfo
    {
        public string Name { get; private set; }
        public string Specialization { get; private set; }

        private DoctorInfo() { }

        public DoctorInfo(string name, string specialization)
        {
            Name = name;
            Specialization = specialization;
        }
    }
}
