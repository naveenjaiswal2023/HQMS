using MediatR;
using PaymentService.Application.Commands;
using PaymentService.Application.Exceptions;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Domain.Models.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Handlers.Commands
{
    public class InitiateRegistrationPaymentCommandHandler : IRequestHandler<InitiateRegistrationPaymentCommand, InitiatePaymentResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IPaymentGateway _paymentGateway;
        private readonly IPaymentGateway _paymentGateway;

        public InitiateRegistrationPaymentCommandHandler(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
        }

        public async Task<InitiatePaymentResult> Handle(InitiateRegistrationPaymentCommand request, CancellationToken cancellationToken)
        {
            // Get registration fee
            var registrationFee = await _unitOfWork.RegistrationFees.GetByFeeTypeAsync(request.FeeType);
            if (registrationFee == null || !registrationFee.IsActive)
            {
                throw new NotFoundException($"Registration fee type '{request.FeeType}' not found or inactive");
            }

            // Create payment record
            var payment = new Payment(
                request.PatientId,
                request.Amount,
                request.Currency,
                request.PaymentMethod,
                request.FeeType,
                request.PayerName,
                request.CustomerPhone,
                request.CustomerEmail
            );

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveAsync(cancellationToken);

            // Initiate payment with gateway
            var paymentRequest = new PaymentInitiationRequest(
                payment.TransactionId,
                payment.Amount,
                payment.Currency,
                payment.PaymentMethod,
                request.CustomerEmail,
                request.CustomerPhone,
                $"Hospital Registration Fee - {request.FeeType}"
            );

            payment.MarkAsProcessing();
            var gatewayResponse = await _paymentGateway.InitiatePaymentAsync(paymentRequest);

            if (!gatewayResponse.IsSuccess)
            {
                payment.MarkAsFailed(gatewayResponse.ErrorMessage);
            }

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveAsync(cancellationToken);

            if (!gatewayResponse.IsSuccess)
            {
                throw new InvalidOperationException($"Payment initiation failed: {gatewayResponse.ErrorMessage}");
            }

            return new InitiatePaymentResult(
                payment.Id,
                payment.TransactionId,
                gatewayResponse.PaymentUrl,
                payment.Amount,
                payment.Currency
            );
        }
    }
}
