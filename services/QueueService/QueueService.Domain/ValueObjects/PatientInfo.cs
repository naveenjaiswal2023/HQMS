using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.ValueObjects
{
    public class PatientInfo
    {
        public Guid Id { get; }
        public string FullName { get; }

        public PatientInfo()
        {
            
        }

        public PatientInfo(Guid id, string fullName)
        {
            Id = id;
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        }
    }
}
