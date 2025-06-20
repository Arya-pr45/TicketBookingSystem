using AutoMapper;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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
                            : new List<int>()))  // Return empty list if SeatIds is null
                .ReverseMap()
                .ForMember(dest => dest.SeatIds,
                    opt => opt.MapFrom(src =>
                        src.SeatIds != null && src.SeatIds.Any()
                            ? string.Join(",", src.SeatIds)  // Join the list back to a comma-separated string
                            : null));  // Return null if SeatIds is empty or null
        }
        //Here ParseSeatIds is handling null or non numeric values
        private static List<int> ParseSeatIds(string seatIds)
        {
            if (string.IsNullOrWhiteSpace(seatIds))
            {
                return new List<int>();  // Return empty list if the input is null or empty
            }

            // Attempt to parse each seatId and filter out invalid ones
            return seatIds.Split(',')
                          .Where(s => int.TryParse(s, out _))  // Filter out non-numeric values
                          .Select(s => int.Parse(s))  // Parse valid numeric values
                          .ToList();
        }
    }
}
