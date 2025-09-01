﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Commands
{
    public record VerifyPaymentCommand(string TransactionId) : IRequest<PaymentVerificationResult>;

}
