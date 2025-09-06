using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInfrastructure.DTO
{
    public record HospitalDto(
    Guid Id,
    string Name,
    string Address,
    string City,
    string Phone,
    string Email
    );
}
