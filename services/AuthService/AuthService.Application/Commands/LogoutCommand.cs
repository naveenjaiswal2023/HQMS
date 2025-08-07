﻿using AuthService.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Commands
{
    public class LogoutCommand : IRequest<Result<bool>>
    {
    }
}
