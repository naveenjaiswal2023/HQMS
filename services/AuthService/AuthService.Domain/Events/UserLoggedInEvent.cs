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
    public class UserLoggedInEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid UserId { get; }
        public string Username { get; }
        public DateTime LoginTime { get; }
        public UserLoggedInEvent(Guid userId, string username, DateTime loginTime)
        {
            UserId = userId;
            Username = username;
            LoginTime = loginTime;
        }
    }
}
