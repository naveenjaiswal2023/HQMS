using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interface
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
