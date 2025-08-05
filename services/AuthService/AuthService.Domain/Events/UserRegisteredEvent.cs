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

        {
            UserId = userId;
            Email = email;
            Role = role;
        }
    }

}
