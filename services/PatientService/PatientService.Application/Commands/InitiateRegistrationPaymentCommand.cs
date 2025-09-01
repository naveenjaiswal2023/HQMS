using MediatR;
using PatientService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Commands
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
