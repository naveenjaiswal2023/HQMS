using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Events
{
    
    public class UserRegisteredEvent : INotification
    {
        public string UserId { get; set; }
        public string Email { get; set; }

        public UserRegisteredEvent(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }

}
