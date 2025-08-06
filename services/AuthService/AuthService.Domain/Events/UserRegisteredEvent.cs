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

    public class UserRegisteredEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string Username { get; }
        public string Role { get; }
        public string ConfirmUrl { get; }
        public DateTime RegisteredAt { get; }

        public UserRegisteredEvent(Guid userId, string email, string username, string role, string confirmUrl, DateTime registeredAt)
        {
            UserId = userId;
            Email = email;
            Username = username;
            Role = role;
            ConfirmUrl = confirmUrl;
            RegisteredAt = registeredAt;
        }
    }

}
