using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Domain.Interfaces;
using PatientService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientDbContext _context;

        public PatientRepository(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Patients.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<Patient>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Patients.ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Patient entity, CancellationToken cancellationToken = default)
        {
            await _context.Patients.AddAsync(entity, cancellationToken);
            
        }

        public async Task UpdateAsync(Patient entity, CancellationToken cancellationToken = default)
        {
            _context.Patients.Update(entity);
        }

        public Task DeleteAsync(Patient entity, CancellationToken cancellationToken = default)
        {
            _context.Patients.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _context.Patients
                .Where(p => p.FirstName.Contains(searchTerm) ||
                            p.LastName.Contains(searchTerm) ||
                            p.Email.Contains(searchTerm))
                .ToListAsync(cancellationToken);
        }

        public async Task<Patient?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber, cancellationToken);
        }
    }
}
