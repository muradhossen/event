using API.Dto;
using API.Entities;
using API.Extentions;
using AutoMapper;
using System.Linq;

namespace API.Helpers
{
    public class AutomapperProfile :Profile
    {
        public AutomapperProfile()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x=>x.IsMain).Url))
                .ForMember(dest => dest.Age,opt => opt.MapFrom(src=> src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<Message, MessageDto>()
                .ForMember(des => des.SenderPhotoUrl, 
                opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(c => c.IsMain).Url))
                .ForMember(des => des.RecipientPhotoUrl,
                opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(c => c.IsMain).Url));
        }
    }
}
