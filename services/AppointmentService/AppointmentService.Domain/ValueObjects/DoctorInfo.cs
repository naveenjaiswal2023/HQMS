using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Domain.ValueObjects
{
    public class DoctorInfo
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Specialization { get; }

        private DoctorInfo() { }

        public DoctorInfo(Guid id, string name, string specialization)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Specialization = specialization ?? string.Empty;
        }
    }
}
