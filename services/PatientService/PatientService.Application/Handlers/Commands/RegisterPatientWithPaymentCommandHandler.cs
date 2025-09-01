using MediatR;
using PatientService.Application.Commands;
using PatientService.Application.Interfaces;
using PatientService.Application.Models;
using PatientService.Domain.Entities;
using PatientService.Domain.Enums;
using PatientService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

public class RegisterPatientWithPaymentCommandHandler
    : IRequestHandler<RegisterPatientWithPaymentCommand, PatientRegistrationResult>
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

    public async Task<PatientRegistrationResult> Handle(
        RegisterPatientWithPaymentCommand request,
        CancellationToken cancellationToken)
    {
        // 🔎 Check by phone number
        var existingPatient = await _unitOfWork.PatientRepository
            .GetByPhoneNumberAsync(request.PhoneNumber);

        if (existingPatient != null)
            throw new InvalidOperationException("Patient with this phone number already exists.");

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
            // 💵 Cash flow - no online payment
            paymentResult = new PaymentResult
            {
                PaymentId = Guid.NewGuid(),
                TransactionId = null,
                PaymentUrl = null,
                Amount = request.RegistrationFeeType == RegistrationFeeType.Consultation.ToString() ? 500 : 1000, // example
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
                    request.RegistrationFeeType,             // string or enum.ToString()
                    500.00m,                                 // amount (example: registration fee)
                    request.PaymentMethod,                   // PaymentMethod enum
                    $"{request.FirstName} {request.LastName}", // payerName
                    request.Email,                           // may be null, handled by client
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
                throw new ApplicationException("Payment initiation failed.", ex);
            }
        }

        // update payment status
        await _unitOfWork.PatientRepository.UpdateAsync(patient);
        await _unitOfWork.SaveAsync(cancellationToken);

        var paymentInfo = new PaymentInfo(
            paymentResult.PaymentId,
            paymentResult.TransactionId,
            paymentResult.PaymentUrl,
            paymentResult.Amount,
            paymentResult.Currency
        );

        return new PatientRegistrationResult(
            patient.Id,
            patient.UHID,
            patient.RegistrationStatus,
            paymentInfo
        );
    }
}
