using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.Repositories
{
    public class RegistrationFeeRepository : IRegistrationFeeRepository
    {
        private readonly PaymentDbContext _context;

        public RegistrationFeeRepository(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task<RegistrationFee> GetByIdAsync(Guid id)
        {
            return await _context.RegistrationFees.FindAsync(id);
        }

        public async Task<RegistrationFee> AddAsync(RegistrationFee fee)
        {
            _context.RegistrationFees.Add(fee);
            await _context.SaveChangesAsync();
            return fee;
        }

        public void Update(RegistrationFee fee)
        {
            _context.RegistrationFees.Update(fee);
        }

        public async Task<RegistrationFee> GetByFeeTypeAsync(string feeType)
        {
            return await _context.RegistrationFees
                .FirstOrDefaultAsync(f => f.FeeType == feeType);
        }

        public async Task<IEnumerable<RegistrationFee>> GetActiveFeesAsync()
        {
            return await _context.RegistrationFees
                .Where(f => f.IsActive)
                .ToListAsync();
        }
    }
}
