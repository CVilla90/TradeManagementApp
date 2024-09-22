using Microsoft.AspNetCore.Mvc.RazorPages;
using TradeManagementApp.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class ViewDocumentModel : PageModel
{
    private readonly TradeContext _context;

    public ViewDocumentModel(TradeContext context)
    {
        _context = context;
    }

    // Made Document nullable to handle null cases more safely
    public Document? Document { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Document = await _context.Documents.FindAsync(id);
        
        // Check for null to prevent potential issues
        if (Document == null) 
            return NotFound();
        
        return Page();
    }
}

// Relative path: TradeManagementApp\Pages\ViewDocument.cshtml.cs
