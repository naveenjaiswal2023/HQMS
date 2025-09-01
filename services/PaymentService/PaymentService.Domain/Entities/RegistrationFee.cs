using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain.Entities
{
    public class RegistrationFee
    {
        public Guid Id { get; private set; }
        public string FeeType { get; private set; } // "NEW_PATIENT", "CONSULTATION", "EMERGENCY"
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected RegistrationFee() { }

        public RegistrationFee(string feeType, decimal amount, string currency, string description)
        {
            Id = Guid.NewGuid();
            FeeType = feeType ?? throw new ArgumentNullException(nameof(feeType));
            Amount = amount;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            Description = description;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
