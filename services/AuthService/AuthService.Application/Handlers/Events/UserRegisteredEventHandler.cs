//using AuthService.Domain.Events;
//using AuthService.Domain.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AuthService.Application.Handlers.Events
//{
//    public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
//    {
//        private readonly IDomainEventPublisher _publisher;

//        public UserRegisteredEventHandler(IDomainEventPublisher publisher)
//        {
//            _publisher = publisher;
//        }

//        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
//        {
//            await _publisher.PublishAsync(notification, cancellationToken);
//        }
//    }
//}
