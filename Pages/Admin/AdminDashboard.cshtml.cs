using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TradeManagementApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AdminDashboardModel : PageModel
{
    private readonly TradeContext _context;

    public AdminDashboardModel(TradeContext context)
    {
        _context = context;
    }

    public List<IdentityUser> Users { get; set; } = new List<IdentityUser>();  // List of all users
    public List<Document> Documents { get; set; } = new List<Document>();      // Documents of the selected user
    public IdentityUser? SelectedUser { get; set; }                            // The user whose documents are shown

    public async Task OnGetAsync(string? userId)
    {
        // Load all users
        Users = await _context.Users.ToListAsync();

        // If a user is selected (via userId), load their documents
        if (!string.IsNullOrEmpty(userId))
        {
            SelectedUser = await _context.Users.FindAsync(userId);
            if (SelectedUser != null)
            {
                Documents = await _context.Documents
                    .Where(d => d.UserId == userId)
                    .ToListAsync();  // Load documents of the selected user
            }
        }
        else
        {
            // If no user is selected, no documents are shown by default
            Documents = new List<Document>();
        }
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string Status)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        document.Status = Status;
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    // Method to determine badge class for AI score
    public string GetAIScoreClass(float score)
    {
        if (score >= 75) return "badge-success";  // Green for high score
        if (score >= 50) return "badge-warning";  // Yellow for moderate score
        return "badge-danger";                    // Red for low score
    }
}

// TradeManagementApp\Pages\AdminDashboard.cshtml.cs