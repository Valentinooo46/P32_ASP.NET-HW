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
        CreateMap<CountryEntity, CountryItemModel>();

        CreateMap<CountryCreateModel, CountryEntity>()
            .ForMember(x=>x.Image, opt=>opt.Ignore());

        CreateMap<CountryUpdateModel, CountryEntity>()
            .ForMember(x => x.Image, opt => opt.Ignore());
    }
}
