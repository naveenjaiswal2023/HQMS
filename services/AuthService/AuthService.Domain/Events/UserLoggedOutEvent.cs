using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Events
{
    public class UserLoggedOutEvent : INotification
    {
        public string UserId { get; }
        public DateTime LoggedOutAt { get; }

        public UserLoggedOutEvent(string userId, DateTime loggedOutAt)
        {
            UserId = userId;
            LoggedOutAt = loggedOutAt;
        }
    }
}
