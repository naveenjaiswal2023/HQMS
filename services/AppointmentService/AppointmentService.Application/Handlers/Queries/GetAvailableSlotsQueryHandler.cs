using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppointmentService.Application.Queries;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using AppointmentService.Domain.ValueObjects;
using MediatR;

namespace AppointmentService.Application.Handlers.Queries
{
    public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, List<TimeSlot>>
    {
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;

        public GetAvailableSlotsQueryHandler(IDoctorScheduleRepository doctorScheduleRepository)
        {
            _doctorScheduleRepository = doctorScheduleRepository ?? throw new ArgumentNullException(nameof(doctorScheduleRepository));
        }

        public async Task<List<TimeSlot>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
        {
            // Get the schedule for this doctor on the given day
            var schedule = await _doctorScheduleRepository.GetByDoctorAndDayAsync(
                request.DoctorId,
                request.Date.DayOfWeek,
                cancellationToken
            );

            if (schedule == null || !schedule.IsActive)
                return new List<TimeSlot>();

            // Generate available slots from schedule
            var slots = schedule.GetAvailableTimeSlots();

            return slots;
        }
    }
}
