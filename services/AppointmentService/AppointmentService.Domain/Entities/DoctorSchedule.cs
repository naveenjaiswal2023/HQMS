using AppointmentService.Domain.Common;
using AppointmentService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Domain.Entities
{
    public class DoctorSchedule : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid DoctorId { get; private set; } // Reference to Doctor Service
        public DayOfWeek DayOfWeek { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public bool IsActive { get; private set; }
        public int SlotDurationMinutes { get; private set; }

        private DoctorSchedule() { } // EF Core

        public static DoctorSchedule Create(Guid doctorId, DayOfWeek dayOfWeek,
            TimeSpan startTime, TimeSpan endTime, int slotDurationMinutes = 30)
        {
            return new DoctorSchedule
            {
                Id = Guid.NewGuid(),
                DoctorId = doctorId,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                IsActive = true,
                SlotDurationMinutes = slotDurationMinutes
            };
        }

        public List<TimeSlot> GetAvailableTimeSlots()
        {
            var slots = new List<TimeSlot>();
            var current = StartTime;
            var slotDuration = TimeSpan.FromMinutes(SlotDurationMinutes);

            while (current.Add(slotDuration) <= EndTime)
            {
                slots.Add(new TimeSlot(current, current.Add(slotDuration)));
                current = current.Add(slotDuration);
            }

            return slots;
        }
    }

}
