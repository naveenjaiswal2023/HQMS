using AppointmentService.Application.DTOs;

namespace AppointmentService.Application.Mappers
{
    public static class HospitalMapper
    {
        public static HospitalDto ToApplicationDto(this SharedInfrastructure.DTO.HospitalDto dto)
        {
            return new HospitalDto(
                dto.Id,
                dto.Name,
                dto.Address,
                dto.City,
                dto.Phone,
                dto.Email
            );
        }
    }
}