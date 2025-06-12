using AutoMapper;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Venue, VenueDto>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Address))
                .ReverseMap();

            CreateMap<Event, EventDto>().ReverseMap();
            CreateMap<Booking, BookingDto>().ReverseMap();

        }
    }
}
