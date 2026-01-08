using AutoMapper;
using Core.Models.Location.City;
using Domain.Entities.Location;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Core.Mappers;

public class CityMapper : Profile
{
    public CityMapper()
    {
        CreateMap<CityEntity, CityItemModel>()
            .ForMember(x => x.Country, opt => opt.MapFrom(x => x.Country.Name))
            .ForMember(dest => dest.Image, opt => opt.MapFrom<CityImageUrlResolver>());

        CreateMap<CityCreateModel, CityEntity>()
            .ForMember(x=>x.Image, opt=>opt.Ignore());
    }
}

public class CityImageUrlResolver : IValueResolver<CityEntity, CityItemModel, string?>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public CityImageUrlResolver(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string? Resolve(CityEntity source, CityItemModel destination, string? destMember, ResolutionContext context)
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
