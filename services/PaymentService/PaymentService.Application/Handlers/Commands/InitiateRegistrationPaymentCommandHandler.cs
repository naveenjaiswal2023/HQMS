using MediatR;
using PaymentService.Application.Commands;
using PaymentService.Application.Common.Models;
using PaymentService.Application.Exceptions;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Domain.Models.Payments;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentService.Application.Handlers.Commands
{
    public class InitiateRegistrationPaymentCommandHandler
        : IRequestHandler<InitiateRegistrationPaymentCommand, Result<InitiatePaymentResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;

        public InitiateRegistrationPaymentCommandHandler(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
        }

        public async Task<Result<InitiatePaymentResult>> Handle(
            InitiateRegistrationPaymentCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validate fee type
                var registrationFee = await _unitOfWork.RegistrationFees.GetByFeeTypeAsync(request.FeeType);
                if (registrationFee == null || !registrationFee.IsActive)
                {
                    return Result<InitiatePaymentResult>.Failure(
                        $"Registration fee type '{request.FeeType}' not found or inactive");
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

                // ✅ Prepare gateway request
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
                    _unitOfWork.Payments.Update(payment);
                    await _unitOfWork.SaveAsync(cancellationToken);

                    return Result<InitiatePaymentResult>.Failure(
                        $"Payment initiation failed: {gatewayResponse.ErrorMessage}");
                }

                // ✅ Update payment status and save
                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.SaveAsync(cancellationToken);

                var result = new InitiatePaymentResult(
                    payment.Id,
                    payment.TransactionId,
                    gatewayResponse.PaymentUrl,
                    payment.Amount,
                    payment.Currency
                );

                return Result<InitiatePaymentResult>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<InitiatePaymentResult>.Failure($"Unexpected error: {ex.Message}");
            }
        }
    }
}
