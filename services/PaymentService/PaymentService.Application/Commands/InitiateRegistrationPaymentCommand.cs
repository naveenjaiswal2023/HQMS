using MediatR;
using PaymentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Commands
{
    public record InitiateRegistrationPaymentCommand(
    Guid PatientId,
    string FeeType,
    decimal Amount,
    string Currency,
    PaymentMethod PaymentMethod,
    string? PayerName,
    string? CustomerEmail,
    string? CustomerPhone
    ) : IRequest<InitiatePaymentResult>;

}
