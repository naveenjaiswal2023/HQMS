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
        public string UserName { get; set; }
        public string Role { get; set; }
        //public DateTime RegisteredOn { get; set; }

        public UserRegisteredEvent(string userId, string email, string userName, string role, DateTime registeredOn)
        {
            UserId = userId;
            Email = email;
            UserName = userName;
            Role = role;
            //RegisteredOn = registeredOn;
        }
    }

}
