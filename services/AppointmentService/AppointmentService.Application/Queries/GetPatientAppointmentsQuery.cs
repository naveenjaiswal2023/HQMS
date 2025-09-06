
using AppointmentService.Domain.Entities;
using MediatR;

public class GetPatientAppointmentsQuery : IRequest<IEnumerable<Appointment>>
{
    public GetPatientAppointmentsQuery(Guid patientId, bool? upcomingOnly = null)
    {
        PatientId = patientId;
        UpcomingOnly = upcomingOnly;
    }

    public Guid PatientId { get; }
    public bool? UpcomingOnly { get; }
}
