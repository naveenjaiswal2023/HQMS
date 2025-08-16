using QueueService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QueueService.Application.DTOs
{
    public class QueueDetailsDto
    {
        public Guid Id { get; set; }
        public string QueueNumber { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public QueueStatus Status { get; set; }
        public int Position { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? CalledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? SkippedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
    }
}
