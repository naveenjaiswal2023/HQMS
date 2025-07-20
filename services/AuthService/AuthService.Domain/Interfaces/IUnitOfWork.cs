using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
        // Expose repositories here if needed:
        // IPatientRepository Patients { get; }
        // IDoctorRepository Doctors { get; }
    }
}
