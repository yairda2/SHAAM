using AutoMapper;
using UserManagementAPI.Models;

namespace UserManagementAPI.Mappers
{
    /// <summary>
    /// AutoMapper profile for object-to-object mappings
    /// Defines how entities are mapped to DTOs and vice versa
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map User entity to UserDto
            CreateMap<User, UserDto>();

            // Map CreateUserDto to User entity
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow));

            // Map UpdateUserDto to User entity
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow));
        }
    }
}
