using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInfrastructure.DTO
{
    public record DoctorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Specialization,
    Guid HospitalId,
    string Email,
    string Phone
    );
}
