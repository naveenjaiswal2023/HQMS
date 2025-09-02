using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> GetByIdAsync(Guid id);
        Task<Payment> GetByTransactionIdAsync(string transactionId);
        Task<IEnumerable<Payment>> GetByPatientIdAsync(Guid patientId);
        Task<Payment> AddAsync(Payment payment);
        void Update(Payment payment);
    }
}
