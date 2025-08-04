using AuthService.Domain.Common;
using AuthService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Events
{
    public class UserLoggedOutEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid UserId { get; }
        public string Username { get; }
        public DateTime LogoutTime { get; }

        public UserLoggedOutEvent(Guid userId, string username, DateTime logoutTime)
        {
            UserId = userId;
            Username = username;
            LogoutTime = logoutTime;
        }
    }
}
