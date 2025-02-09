using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDto>().IncludeMembers(src => src.Item);
        CreateMap<Item, AuctionDto>();
        CreateMap<CreateAuctionDto, Auction>().ForMember(dest => dest.Item, opt => opt.MapFrom(src => src));
        CreateMap<CreateAuctionDto, Item>();
        CreateMap<AuctionDto, AuctionCreated>();
        CreateMap<Auction, AuctionUpdated>().IncludeMembers(src => src.Item);
        CreateMap<Item, AuctionUpdated>();
    }
}
