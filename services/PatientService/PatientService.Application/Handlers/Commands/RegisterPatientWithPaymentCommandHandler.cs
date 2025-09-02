using MediatR;
using PatientService.Application.Commands;
using PatientService.Application.Common.Models;
using PatientService.Application.Interfaces;
using PatientService.Application.Models;
using PatientService.Domain.Entities;
using PatientService.Domain.Enums;
using PatientService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

public class RegisterPatientWithPaymentCommandHandler
    : IRequestHandler<RegisterPatientWithPaymentCommand, Result<PatientRegistrationResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentServiceClient _paymentServiceClient;

    public RegisterPatientWithPaymentCommandHandler(
        IUnitOfWork unitOfWork,
        IPaymentServiceClient paymentServiceClient)
    {
        _unitOfWork = unitOfWork;
        _paymentServiceClient = paymentServiceClient;
    }

    public async Task<Result<PatientRegistrationResult>> Handle(
        RegisterPatientWithPaymentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 🔎 (Optional) Check if patient exists by phone
            // var existingPatient = await _unitOfWork.PatientRepository
            //     .GetByPhoneNumberAsync(request.PhoneNumber);
            //
            // if (existingPatient != null)
            //     return Result<PatientRegistrationResult>.Failure("Patient with this phone number already exists.");

            // 🆕 Create new patient
            var patient = new Patient(
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.Gender,
                request.PhoneNumber
            );

            await _unitOfWork.PatientRepository.AddAsync(patient);
            await _unitOfWork.SaveAsync(cancellationToken);

            PaymentResult paymentResult;

            if (request.PaymentMethod == PaymentMethod.Cash)
            {
                // 💵 Cash flow
                paymentResult = new PaymentResult
                {
                    PaymentId = Guid.NewGuid(),
                    TransactionId = null,
                    PaymentUrl = null,
                    Amount = request.RegistrationFeeType == RegistrationFeeType.Consultation.ToString() ? 500 : 1000,
                    Currency = "INR"
                };

                patient.MarkPaymentCompleted();
            }
            else
            {
                // 🌐 Online methods
                try
                {
                    var initiateResult = await _paymentServiceClient.InitiateRegistrationPaymentAsync(
                        patient.Id,
                        request.RegistrationFeeType,
                        500.00m, // Example fixed fee
                        request.PaymentMethod,
                        $"{request.FirstName} {request.LastName}",
                        request.Email,
                        request.PhoneNumber
                    );

                    paymentResult = new PaymentResult
                    {
                        PaymentId = initiateResult.PaymentId,
                        TransactionId = initiateResult.TransactionId,
                        PaymentUrl = initiateResult.PaymentUrl,
                        Amount = initiateResult.Amount,
                        Currency = initiateResult.Currency
                    };

                    patient.MarkPaymentProcessing();
                }
                catch (Exception ex)
                {
                    return Result<PatientRegistrationResult>.Failure($"Payment initiation failed: {ex.Message}");
                }
            }

            await _unitOfWork.PatientRepository.UpdateAsync(patient);
            await _unitOfWork.SaveAsync(cancellationToken);

            var paymentInfo = new PaymentInfo(
                paymentResult.PaymentId,
                paymentResult.TransactionId,
                paymentResult.PaymentUrl,
                paymentResult.Amount,
                paymentResult.Currency
            );

            var registrationResult = new PatientRegistrationResult(
                patient.Id,
                patient.UHID,
                patient.RegistrationStatus,
                paymentInfo
            );

            return Result<PatientRegistrationResult>.Success(registrationResult);
        }
        catch (Exception ex)
        {
            return Result<PatientRegistrationResult>.Failure($"Unexpected error: {ex.Message}");
        }
    }
}
