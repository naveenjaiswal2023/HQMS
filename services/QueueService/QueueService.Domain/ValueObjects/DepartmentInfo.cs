using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.ValueObjects
{
    public class DepartmentInfo
    {
        public Guid Id { get; }
        public string Name { get; }
        public Guid HospitalId { get; }

        public DepartmentInfo(Guid id, string name, Guid hospitalId)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            HospitalId = hospitalId;
        }
    }
}
