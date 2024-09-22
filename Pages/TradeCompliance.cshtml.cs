using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TradeManagementApp.Data;
using System.Threading.Tasks;

namespace TradeManagementApp.Pages
{
    public class TradeComplianceModel : PageModel
    {
        private readonly TradeContext _context;

        public TradeComplianceModel(TradeContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string? TradeData { get; set; } // Nullable to avoid warnings
        public string? ComplianceMessage { get; set; } // Nullable to avoid warnings
        public string? UserTradeData { get; set; } // Added for displaying user input

        public void OnGet()
        {
            ComplianceMessage = "Welcome to the Trade Compliance Dashboard!";
        }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Save trade data to the database
        var tradeEntry = new TradeData { TradeInfo = TradeData ?? string.Empty };
        _context.Trades.Add(tradeEntry);
        await _context.SaveChangesAsync();

        // Use TempData to store values between requests
        TempData["ComplianceMessage"] = "Data submitted and saved successfully!";
        TempData["UserTradeData"] = $"You entered: {TradeData}";

        return RedirectToPage();
    }
    }
}
