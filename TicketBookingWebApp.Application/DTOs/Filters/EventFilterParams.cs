using System;
using System.ComponentModel.DataAnnotations;

namespace TicketBookingWebApp.Application.DTOs.Filters
{
    public class EventFilterParams
    {
        public string? Keyword { get; set; }

        public string? EventType { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        [Range(0, 1000000, ErrorMessage = "Minimum price must be at least 0.")]
        public decimal? MinPrice { get; set; }

        [Range(0, 1000000, ErrorMessage = "Maximum price must be at least 0.")]
        public decimal? MaxPrice { get; set; }

        public bool? OnlyAvailable { get; set; }

        public string? SortBy { get; set; }
    }
}
