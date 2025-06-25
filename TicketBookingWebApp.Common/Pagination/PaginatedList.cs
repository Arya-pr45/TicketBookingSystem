using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TicketBookingWebApp.Common.Pagination
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }       // Current page
        public int TotalPages { get; private set; }      // Total number of pages

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();                  // Total items
            var items = await source.Skip((pageIndex - 1) * pageSize) // OFFSET calculation
                                    .Take(pageSize)                  // LIMIT number of items
                                    .ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }


}
