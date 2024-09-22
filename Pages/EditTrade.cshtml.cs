using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TradeManagementApp.Data;
using System.Threading.Tasks;

namespace TradeManagementApp.Pages
{
    public class EditTradeModel : PageModel
    {
        private readonly TradeContext _context;

        public EditTradeModel(TradeContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TradeData TradeData { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            TradeData = await _context.Trades.FindAsync(id);

            if (TradeData == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var trade = await _context.Trades.FindAsync(id);

            if (trade == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync(
                trade, 
                "TradeData", // Prefix matches the form input name prefix in the view
                t => t.TradeInfo))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/TradeList");
            }

            return Page();
        }
    }
}
