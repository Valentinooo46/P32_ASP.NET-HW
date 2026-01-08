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
            .ForMember(x => x.Country, opt => opt.MapFrom(x => x.Country.Name));

        CreateMap<CityCreateModel, CityEntity>()
            .ForMember(x=>x.Image, opt=>opt.Ignore());
    }
}
