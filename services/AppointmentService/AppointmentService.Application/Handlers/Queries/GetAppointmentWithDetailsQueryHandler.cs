using AppointmentService.Application.Exceptions;
using AppointmentService.Application.Queries;
using AppointmentService.Domain.Interfaces;
using MediatR;
using SharedInfrastructure.ExternalServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Handlers.Queries
{
    public class GetAppointmentWithDetailsQueryHandler : IRequestHandler<GetAppointmentWithDetailsQuery, AppointmentWithDetailsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientServiceClient _patientService;
        private readonly IDoctorServiceClient _doctorService;
        private readonly IHospitalServiceClient _hospitalService;

        public GetAppointmentWithDetailsQueryHandler(
            IUnitOfWork unitOfWork,
            IPatientServiceClient patientService,
            IDoctorServiceClient doctorService,
            IHospitalServiceClient hospitalService)
        {
            _unitOfWork = unitOfWork;
            _patientService = patientService;
            _doctorService = doctorService;
            _hospitalService = hospitalService;
        }

        public async Task<AppointmentWithDetailsResponse> Handle(GetAppointmentWithDetailsQuery request, CancellationToken cancellationToken)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken);

            if (appointment == null)
                throw new NotFoundException($"Appointment with ID {request.Id} not found");

            // Fetch related data from external services
            var patientTask = _patientService.GetPatientAsync(appointment.PatientId, cancellationToken);
            var doctorTask = _doctorService.GetDoctorAsync(appointment.DoctorId, cancellationToken);
            var hospitalTask = _hospitalService.GetHospitalAsync(appointment.HospitalId, cancellationToken);

            await Task.WhenAll(patientTask, doctorTask, hospitalTask);

            var patient = await patientTask ?? throw new NotFoundException($"Patient with ID {appointment.PatientId} not found");
            var doctor = await doctorTask ?? throw new NotFoundException($"Doctor with ID {appointment.DoctorId} not found");
            var hospital = await hospitalTask ?? throw new NotFoundException($"Hospital with ID {appointment.HospitalId} not found");

            return new AppointmentWithDetailsResponse(
                appointment.Id,
                appointment.Title,
                appointment.Description,
                appointment.ScheduledDate,
                appointment.EndDateTime,
                appointment.Status,
                appointment.Type,
                appointment.Location,
                appointment.Notes,
                appointment.Fee,
                appointment.IsPaid,
                patient,
                doctor,
                hospital,
                appointment.QueueId,
                appointment.PaymentId,
                appointment.CreatedAt,
                appointment.UpdatedAt);
        }
    }
}
