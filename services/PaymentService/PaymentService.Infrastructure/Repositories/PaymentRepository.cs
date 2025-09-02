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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentRepository(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> GetByIdAsync(Guid id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task<Payment> GetByTransactionIdAsync(string transactionId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<IEnumerable<Payment>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Payments.Where(p => p.PatientId == patientId).ToListAsync();
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public void Update(Payment payment)
        {
            _context.Payments.Update(payment);
        }
    }
}
