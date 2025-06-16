using AutoMapper;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Venue));

            CreateMap<Venue, VenueDto>().ReverseMap();
            CreateMap<EventDto, Event>().ReverseMap();
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.SeatIds,
                    opt => opt.MapFrom(src =>
                        src.SeatIds != null
                            ? ParseSeatIds(src.SeatIds)
                            : new List<int>()))
                .ReverseMap()
                .ForMember(dest => dest.SeatIds,
                    opt => opt.MapFrom(src =>
                        src.SeatIds != null
                            ? string.Join(",", src.SeatIds)
                            : null));
        }

        private static List<int> ParseSeatIds(string seatIds)
        {
            return seatIds.Split(',').Select(s => int.Parse(s)).ToList();
        }
    }
}
