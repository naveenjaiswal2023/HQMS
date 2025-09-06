using AppointmentService.Application.Commands;
using AppointmentService.Application.DTOs;
using AppointmentService.Application.Exceptions;
using AppointmentService.Domain.Common;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Enums;
using AppointmentService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedInfrastructure.ExternalServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Handlers.Commands
{
    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, CreateAppointmentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientServiceClient _patientService;
        private readonly IDoctorServiceClient _doctorService;
        private readonly IHospitalServiceClient _hospitalService;
        //private readonly IQueueService _queueService;
        //private readonly IPaymentService _paymentService;
        private readonly ILogger<CreateAppointmentCommandHandler> _logger;

        public CreateAppointmentCommandHandler(
            IUnitOfWork unitOfWork,
            IPatientServiceClient patientService,
            IDoctorServiceClient doctorService,
            IHospitalServiceClient hospitalService,
            //IQueueService queueService,
            //IPaymentService paymentService,
            ILogger<CreateAppointmentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _patientService = patientService;
            _doctorService = doctorService;
            _hospitalService = hospitalService;
            //_queueService = queueService;
            //_paymentService = paymentService;
            _logger = logger;
        }

        public async Task<CreateAppointmentResponse> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            // Validate external entities exist
            var patientExists = await _patientService.PatientExistsAsync(request.PatientId, cancellationToken);
            if (!patientExists)
                throw new NotFoundException($"Patient with ID {request.PatientId} not found");

            var doctorExists = await _doctorService.DoctorExistsAsync(request.DoctorId, cancellationToken);
            if (!doctorExists)
                throw new NotFoundException($"Doctor with ID {request.DoctorId} not found");

            var hospitalExists = await _hospitalService.HospitalExistsAsync(request.HospitalId, cancellationToken);
            if (!hospitalExists)
                throw new NotFoundException($"Hospital with ID {request.HospitalId} not found");

            // Check doctor availability
            var isDoctorAvailable = await _doctorService.IsDoctorAvailableAsync(
                request.DoctorId, request.StartDateTime, request.EndDateTime, cancellationToken);

            if (!isDoctorAvailable)
                throw new ConflictException("Doctor is not available during the requested time slot");

            // Check for conflicting appointments
            var hasConflict = await _unitOfWork.Appointments
                .HasConflictingAppointmentAsync(request.DoctorId, request.StartDateTime, request.EndDateTime, cancellationToken: cancellationToken);

            if (hasConflict)
                throw new ConflictException("Doctor already has an appointment during this time slot");

            // Replace this block:
            // var appointment = Appointment.Create(
            //     request.Title,
            //     request.Description,
            //     request.StartDateTime,
            //     request.EndDateTime,
            //     request.PatientId,
            //     request.DoctorId,
            //     request.HospitalId,
            //     request.Location,
            //     request.Type,
            //     request.Fee,
            //     request.SendReminder,
            //     request.ReminderMinutesBefore);

            // With the following, using correct argument types:
            var appointment = Appointment.Create(
                request.Title,
                request.Description,
                request.StartDateTime.Date,
                request.StartDateTime.TimeOfDay,
                request.PatientId,
                request.DoctorId,
                request.HospitalId,
                request.Location,
                request.Type,
                request.Fee,
                request.SendReminder,
                request.ReminderMinutesBefore);

            await _unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            // Add to queue if not emergency
            if (request.Type != AppointmentType.Emergency)
            {
                try
                {
                    //var queueId = await _queueService.AddToQueueAsync(new AddToQueueRequest(
                    //    appointment.Id,
                    //    request.PatientId,
                    //    request.DoctorId,
                    //    "Appointment",
                    //    request.Type == AppointmentType.Emergency ? 0 : 1
                    //), cancellationToken);

                    //appointment.AssignToQueue(queueId);
                    //await _unitOfWork.SaveAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to add appointment {AppointmentId} to queue", appointment.Id);
                }
            }

            // Initiate payment if fee > 0
            bool requiresPayment = request.Fee > 0;
            if (requiresPayment)
            {
                try
                {
                    //await _paymentService.InitiatePaymentAsync(new InitiatePaymentRequest(
                    //    appointment.Id,
                    //    request.PatientId,
                    //    request.Fee,
                    //    $"Payment for {request.Title}"
                    //), cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to initiate payment for appointment {AppointmentId}", appointment.Id);
                }
            }

            _logger.LogInformation("Appointment created successfully with ID: {AppointmentId}", appointment.Id);

            return new CreateAppointmentResponse(
                appointment.Id,
                appointment.Title,
                appointment.ScheduledDate,
                appointment.EndDateTime,
                appointment.Status,
                appointment.Fee,
                requiresPayment);
        }
    }
}
