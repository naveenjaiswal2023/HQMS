using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PaymentService.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<QueueItem, QueueItemDto>()
            //    .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

            //CreateMap<QueueItem, QueueStatusDto>()
            //    .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        }
    }
}
