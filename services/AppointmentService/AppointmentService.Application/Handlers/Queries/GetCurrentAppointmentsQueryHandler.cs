using AppointmentService.Application.DTOs;
using AppointmentService.Application.Mappers;
using AppointmentService.Application.Queries;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Enums;
using AppointmentService.Domain.Interfaces;
using MediatR;
using SharedInfrastructure.ExternalServices.Interfaces;

using SharedPatientDto = SharedInfrastructure.DTO.PatientDto;
using SharedDoctorDto = SharedInfrastructure.DTO.DoctorDto;
using SharedHospitalDto = SharedInfrastructure.DTO.HospitalDto;

namespace AppointmentService.Application.Handlers.Queries
{
    public class GetCurrentAppointmentsQueryHandler : IRequestHandler<GetCurrentAppointmentsQuery, CurrentAppointmentsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientServiceClient _patientService;
        private readonly IDoctorServiceClient _doctorService;
        private readonly IHospitalServiceClient _hospitalService;

        public GetCurrentAppointmentsQueryHandler(
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

        public async Task<CurrentAppointmentsResponse> Handle(GetCurrentAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var currentAppointments = await _unitOfWork.Appointments.GetCurrentAppointmentsAsync(cancellationToken);
            var todayAppointments = await _unitOfWork.Appointments.GetTodayAppointmentsAsync(cancellationToken);

            var allAppointments = currentAppointments.Concat(todayAppointments).DistinctBy(a => a.Id);

            // Get unique IDs for external service calls
            var patientIds = allAppointments.Select(a => a.PatientId).Distinct().ToList();
            var doctorIds = allAppointments.Select(a => a.DoctorId).Distinct().ToList();
            var hospitalIds = allAppointments.Select(a => a.HospitalId).Distinct().ToList();

            // Fetch data from external services
            var patientsTask = _patientService.GetPatientsAsync(patientIds, cancellationToken);

            var doctorsTask = Task.Run(async () =>
            {
                var doctors = new List<DoctorDto>(); // Application DTO
                foreach (var doctorId in doctorIds)
                {
                    SharedDoctorDto? sharedDoctor = await _doctorService.GetDoctorAsync(doctorId, cancellationToken);
                    if (sharedDoctor != null) doctors.Add(sharedDoctor.ToApplicationDto()); // mapped to Application DTO
                }
                return doctors.AsEnumerable();
            });

            var hospitalsTask = Task.Run(async () =>
            {
                var hospitals = new List<HospitalDto>(); // Application DTO
                foreach (var hospitalId in hospitalIds)
                {
                    SharedHospitalDto? sharedHospital = await _hospitalService.GetHospitalAsync(hospitalId, cancellationToken);
                    if (sharedHospital != null) hospitals.Add(sharedHospital.ToApplicationDto()); // mapped to Application DTO
                }
                return hospitals.AsEnumerable();
            });

            // patients remain SharedInfrastructure DTO
            var patients = (await patientsTask ?? Enumerable.Empty<SharedPatientDto>()).ToDictionary(p => p.Id);
            var doctors = (await doctorsTask).ToDictionary(d => d.Id);
            var hospitals = (await hospitalsTask).ToDictionary(h => h.Id);

            // Map to DTOs
            var currentAppointmentDtos = MapToSummaryDtos(currentAppointments, patients, doctors, hospitals);
            var todayAppointmentDtos = MapToSummaryDtos(todayAppointments, patients, doctors, hospitals);

            return new CurrentAppointmentsResponse(
                currentAppointmentDtos.Count(),
                currentAppointmentDtos.Count(a => a.Status == AppointmentStatus.InProgress),
                todayAppointmentDtos.Count(a => a.Status == AppointmentStatus.Scheduled),
                currentAppointmentDtos,
                todayAppointmentDtos
            );
        }

        private static IEnumerable<AppointmentSummaryDto> MapToSummaryDtos(
            IEnumerable<Appointment> appointments,
            Dictionary<Guid, SharedPatientDto> patients,   // 🔹 Shared DTO
            Dictionary<Guid, DoctorDto> doctors,          // 🔹 Application DTO
            Dictionary<Guid, HospitalDto> hospitals)      // 🔹 Application DTO
        {
            return appointments.Select(a => new AppointmentSummaryDto(
                a.Id,
                a.Title,
                a.ScheduledDate,
                a.EndDateTime,
                a.Status,
                a.Type,
                patients.TryGetValue(a.PatientId, out var patient) ? $"{patient.FirstName} {patient.LastName}" : "Unknown Patient",
                doctors.TryGetValue(a.DoctorId, out var doctor) ? $"Dr. {doctor.FirstName} {doctor.LastName}" : "Unknown Doctor",
                hospitals.TryGetValue(a.HospitalId, out var hospital) ? hospital.Name : "Unknown Hospital",
                a.Location
            ));
        }
    }
}
