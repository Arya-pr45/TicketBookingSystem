//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoMapper;
//using Moq;
//using TicketBookingWebApp.Application.DTOs;
//using TicketBookingWebApp.Application.Interfaces;
//using TicketBookingWebApp.Application.Services;
//using TicketBookingWebApp.Domain.Entities;
//using Xunit;

//public class EventServiceTests
//{
//    private readonly Mock<IEventRepository> _eventRepoMock = new();
//    private readonly Mock<IBookingRepository> _bookingRepoMock = new();
//    private readonly Mock<IUserRepository> _userRepoMock = new();
//    private readonly IMapper _mapper;

//    public EventServiceTests()
//    {
//        var config = new MapperConfiguration(cfg =>
//        {
//            cfg.CreateMap<Event, EventDto>();
//        });
//        _mapper = config.CreateMapper();
//    }

//    [Fact]
//    public async Task GetUpcomingEventsAsync_ReturnsOnlyFutureEvents()
//    {
//        // Arrange
//        var now = DateTime.UtcNow;

//        var events = new List<Event>
//        {
//            new Event { Id = 1, Title = "Past Event", EventDateTime = now.AddDays(-2) },
//            new Event { Id = 2, Title = "Future Event", EventDateTime = now.AddDays(5) }
//        };

//        _eventRepoMock.Setup(repo => repo.GetUpcomingEventsAsync())
//            .ReturnsAsync(events.Where(e => e.EventDateTime >= now).ToList());

//        var service = new EventService(
//            _eventRepoMock.Object,
//            _bookingRepoMock.Object,
//            _userRepoMock.Object,
//            _mapper,
//            Mock.Of<Microsoft.Extensions.Logging.ILogger<EventService>>()
//        );

//        // Act
//        var result = await service.GetUpcomingEventsAsync();

//        // Assert
//        Assert.Single(result); // only 1 future event
//        Assert.Equal("Future Event", result.First().Title);
//    }
//}
