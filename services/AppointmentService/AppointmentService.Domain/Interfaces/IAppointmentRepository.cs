using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Enums;

namespace AppointmentService.Domain.Interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<Appointment>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<List<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);

        Task<List<Appointment>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);

        Task<List<Appointment>> GetByHospitalIdAsync(Guid hospitalId, CancellationToken cancellationToken = default);

        Task<List<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

        Task<List<Appointment>> GetUpcomingAppointmentsAsync(DateTime fromTime, DateTime toTime, CancellationToken cancellationToken = default);

        Task<List<Appointment>> GetCurrentAppointmentsAsync(CancellationToken cancellationToken = default);

        Task<List<Appointment>> GetTodayAppointmentsAsync(CancellationToken cancellationToken = default);

        Task<List<Appointment>> GetUpcomingAppointmentsByPatientAsync(Guid patientId, int days = 30, CancellationToken cancellationToken = default);

        Task<List<Appointment>> GetUpcomingAppointmentsByDoctorAsync(Guid doctorId, int days = 7, CancellationToken cancellationToken = default);

        Task<List<Appointment>> GetAppointmentsDueForReminderAsync(CancellationToken cancellationToken = default);

        Task<bool> HasConflictingAppointmentAsync(Guid doctorId, DateTime startDateTime, DateTime endDateTime, Guid? excludeAppointmentId = null, CancellationToken cancellationToken = default);

        Task<int> GetTotalAppointmentsCountAsync(CancellationToken cancellationToken = default);

        Task<int> GetAppointmentsCountByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken = default);

        Task<List<Appointment>> SearchAppointmentsAsync(string searchTerm, CancellationToken cancellationToken = default);
    }
}
