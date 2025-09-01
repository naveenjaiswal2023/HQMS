using MediatR;
using PaymentService.Application.Commands;
using PaymentService.Application.Exceptions;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Enums;
using PaymentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Handlers.Commands
{
    public class VerifyPaymentCommandHandler : IRequestHandler<VerifyPaymentCommand, PaymentVerificationResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IPatientRegistrationService _registrationService;

        public VerifyPaymentCommandHandler(
            IUnitOfWork unitOfWork,
            IPaymentGateway paymentGateway,
            IPatientRegistrationService registrationService)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
            _registrationService = registrationService;
        }

        public async Task<PaymentVerificationResult> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByTransactionIdAsync(request.TransactionId);
            if (payment == null)
            {
                throw new NotFoundException($"Payment with transaction ID {request.TransactionId} not found");
            }

            var verificationResponse = await _paymentGateway.VerifyPaymentAsync(request.TransactionId);

            if (verificationResponse.IsSuccess && verificationResponse.Status == PaymentStatus.Success)
            {
                payment.MarkAsSuccess(verificationResponse.GatewayResponse);

                // Complete patient registration
                await _registrationService.CompleteRegistrationAsync(payment.PatientId);
            }
            else
            {
                payment.MarkAsFailed(verificationResponse.ErrorMessage ?? "Payment verification failed");
            }

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveAsync(cancellationToken);

            return new PaymentVerificationResult(
                verificationResponse.IsSuccess,
                payment.Status,
                verificationResponse.IsSuccess ? "Payment verified successfully" : verificationResponse.ErrorMessage
            );
        }
    }
}
