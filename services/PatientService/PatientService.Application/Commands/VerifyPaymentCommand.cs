using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Commands
{
    public record VerifyPaymentCommand(string TransactionId) : IRequest<PaymentVerificationResult>;

}
