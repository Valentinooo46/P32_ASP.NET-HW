using AutoMapper;
using Core.Models.Account;
using Domain.Entities.Idenity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Core.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserProfileModel>()
            .ForMember(x => x.FullName, opt => opt.MapFrom(x => $"{x.LastName} {x.FirstName}"))
            .ForMember(x => x.Phone, opt => opt.MapFrom(x => x.PhoneNumber))
            .ForMember(dest => dest.Image, opt => opt.MapFrom<UserImageUrlResolver>());

        CreateMap<UserEntity, UserItemModel>()
            .ForMember(x => x.FullName, opt => opt.MapFrom(x => $"{x.LastName} {x.FirstName}"))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles!.Select(ur => ur.Role.Name).ToList()))
            .ForMember(dest => dest.Image, opt => opt.MapFrom<UserImageUrlResolver>());
        //.ForMember(x => x.Phone, opt => opt.MapFrom(x => x.PhoneNumber));

        CreateMap<GoogleAccountModel, UserEntity>()
            .ForMember(x => x.Image, opt => opt.Ignore())
            .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.Email));
    }
}

public class UserImageUrlResolver : IValueResolver<UserEntity, object, string?>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public UserImageUrlResolver(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string? Resolve(UserEntity source, object destination, string? destMember, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source.Image))
            return null;

        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
            return source.Image;

        var dirImageName = _configuration.GetValue<string>("DirImageName") ?? "images";
        var scheme = request.Scheme;
        var host = request.Host.Value;
        
        return $"{scheme}://{host}/{dirImageName}/{source.Image}";
    }
}
