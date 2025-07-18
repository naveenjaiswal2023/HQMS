using HQMS.QueueService.Domain.Interfaces;
using HQMS.QueueService.Infrastructure.Persistence;
using HQMS.QueueService.Shared.Interfaces;

namespace HQMS.QueueService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _context;
        public IQueueItemRepository QueueItemRepository { get; }
        public IAppointmentRepository AppointmentRepository { get; set; }

        public UnitOfWork(AuthDbContext context, IQueueItemRepository queueItemRepository, IAppointmentRepository appointmentRepository)
        {
            _context = context;
            QueueItemRepository = queueItemRepository;
            AppointmentRepository = appointmentRepository;
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
