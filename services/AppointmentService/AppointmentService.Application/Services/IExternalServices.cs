//using AppointmentService.Application.DTOs;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AppointmentService.Application.Services
//{
//    public interface IPatientService
//    {
//        Task<PatientDto?> GetPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
//        Task<bool> PatientExistsAsync(Guid patientId, CancellationToken cancellationToken = default);
//        Task<IEnumerable<PatientDto>> GetPatientsAsync(IEnumerable<Guid> patientIds, CancellationToken cancellationToken = default);
//    }

//    public interface IDoctorService
//    {
//        Task<DoctorDto?> GetDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);
//        Task<bool> DoctorExistsAsync(Guid doctorId, CancellationToken cancellationToken = default);
//        Task<IEnumerable<DoctorDto>> GetDoctorsByHospitalAsync(Guid hospitalId, CancellationToken cancellationToken = default);
//        Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
//    }

//    public interface IHospitalService
//    {
//        Task<HospitalDto?> GetHospitalAsync(Guid hospitalId, CancellationToken cancellationToken = default);
//        Task<bool> HospitalExistsAsync(Guid hospitalId, CancellationToken cancellationToken = default);
//        Task<IEnumerable<HospitalDto>> GetHospitalsAsync(CancellationToken cancellationToken = default);
//    }

//    public interface IQueueService
//    {
//        Task<Guid> AddToQueueAsync(AddToQueueRequest request, CancellationToken cancellationToken = default);
//        Task RemoveFromQueueAsync(Guid queueId, CancellationToken cancellationToken = default);
//        Task UpdateQueueStatusAsync(Guid queueId, string status, CancellationToken cancellationToken = default);
//    }

//    public interface IPaymentService
//    {
//        Task<PaymentDto?> GetPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
//        Task<bool> IsPaymentCompletedAsync(Guid appointmentId, CancellationToken cancellationToken = default);
//        Task InitiatePaymentAsync(InitiatePaymentRequest request, CancellationToken cancellationToken = default);
//    }

//    public interface INotificationService
//    {
//        Task SendAppointmentReminderAsync(Guid patientId, Guid appointmentId, DateTime appointmentTime, CancellationToken cancellationToken = default);
//        Task SendAppointmentConfirmationAsync(Guid patientId, Guid appointmentId, CancellationToken cancellationToken = default);
//        Task SendAppointmentCancellationAsync(Guid patientId, Guid appointmentId, CancellationToken cancellationToken = default);
//    }
//}
