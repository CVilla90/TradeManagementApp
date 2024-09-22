using Microsoft.AspNetCore.Mvc.RazorPages;
using TradeManagementApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


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
            // Optionally, load documents for all users if no user is selected
            Documents = await _context.Documents.ToListAsync();
        }
    }
}


// TradeManagementApp\Pages\AdminDashboard.cshtml.cs