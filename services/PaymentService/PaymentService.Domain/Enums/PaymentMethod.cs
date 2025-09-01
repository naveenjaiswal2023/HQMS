using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain.Enums
{
    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        UPI,
        NetBanking,
        Wallet,
        Cash
    }
}
