using AppointmentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Mappers
{
    public static class DoctorMapper
    {
        public static DoctorDto ToApplicationDto(this SharedInfrastructure.DTO.DoctorDto dto)
        {
            return new DoctorDto(
                dto.Id,
                dto.FirstName,
                dto.LastName,
                dto.Specialization,
                dto.HospitalId,
                dto.Email,
                dto.Phone
            );
        }
    }
}
