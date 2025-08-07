using System.ComponentModel.DataAnnotations;

namespace AppointmentService.Domain.Entities
{
    public class OutboxMessage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedOn { get; set; }
        public string? Error { get; set; }
    }
}
