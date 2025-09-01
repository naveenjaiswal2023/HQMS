using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Interfaces
{
    public interface IPatientRegistrationService
    {
        Task CompleteRegistrationAsync(Guid patientId);
    }
}
