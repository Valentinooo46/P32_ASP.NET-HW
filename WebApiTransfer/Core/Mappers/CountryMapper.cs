using AutoMapper;
using Core.Models.Location.Country;
using Domain.Entities.Location;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Core.Mappers;

public class CountryMapper : Profile
{
    public CountryMapper()
    {
        CreateMap<CountryEntity, CountryItemModel>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom<ImageUrlResolver>());

        CreateMap<CountryCreateModel, CountryEntity>()
            .ForMember(x=>x.Image, opt=>opt.Ignore());

        CreateMap<CountryUpdateModel, CountryEntity>()
            .ForMember(x => x.Image, opt => opt.Ignore());
    }
}

public class ImageUrlResolver : IValueResolver<CountryEntity, CountryItemModel, string?>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public ImageUrlResolver(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string? Resolve(CountryEntity source, CountryItemModel destination, string? destMember, ResolutionContext context)
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
