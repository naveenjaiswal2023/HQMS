using AppointmentService.Domain.Interfaces;
using AppointmentService.Infrastructure.Repositories;

namespace AppointmentService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppointmentDbContext _context;
        public IAppointmentRepository Appointments { get; }

        public UnitOfWork(AppointmentDbContext context)
        {
            _context = context;
            Appointments = (IAppointmentRepository)new AppointmentRepository(context);
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();

    }
}
