using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TradeManagementApp.Data;
using System.Collections.Generic;
using System.Linq;

namespace TradeManagementApp.Pages
{
    public class TradeListModel : PageModel
    {
        private readonly TradeContext _context;

        public TradeListModel(TradeContext context)
        {
            _context = context;
        }

        public List<TradeData> Trades { get; set; } = new List<TradeData>();
        public string? SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5; // Default value

        public void OnGet(string? searchTerm, string sortOrder, int pageNumber = 1, int pageSize = 5)
        {
            SearchTerm = searchTerm;
            PageSize = pageSize;
            CurrentPage = pageNumber;

            var tradesQuery = _context.Trades.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                tradesQuery = tradesQuery.Where(t => t.TradeInfo.Contains(searchTerm));
            }

            tradesQuery = sortOrder == "desc"
                ? tradesQuery.OrderByDescending(t => t.TradeInfo)
                : tradesQuery.OrderBy(t => t.TradeInfo);

            var totalItems = tradesQuery.Count();
            TotalPages = (int)System.Math.Ceiling(totalItems / (double)PageSize);

            Trades = tradesQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
