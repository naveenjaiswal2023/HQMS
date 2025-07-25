﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Events
{
    public record UserLoggedInEvent(string UserId, DateTime LoginTime) : INotification;
}
