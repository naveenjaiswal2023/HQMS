using PatientService.Domain.Entities;
using QueueService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Domain.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<Patient>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<Patient?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    }
}
