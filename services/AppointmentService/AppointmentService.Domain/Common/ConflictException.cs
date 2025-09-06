using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Domain.Common
{
    public class ConflictException : DomainException
    {
        public ConflictException() { }

        public ConflictException(string message)
            : base(message) { }

        public ConflictException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
