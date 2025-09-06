using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IAppointmentRepository Appointments { get; }
        Task<int> SaveAsync(CancellationToken cancellationToken);
        // Expose repositories here if needed:
        // IPatientRepository Patients { get; }
        // IDoctorRepository Doctors { get; }
    }
}
