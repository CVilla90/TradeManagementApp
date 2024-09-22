using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TradeManagementApp.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class UserDashboardModel : PageModel
{
    private readonly TradeContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public UserDashboardModel(TradeContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public string DocumentName { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? UploadedFile { get; set; }

    public List<Document> Documents { get; set; } = new List<Document>();

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        Documents = await _context.Documents
                                  .Where(d => d.UserId == userId)
                                  .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = _userManager.GetUserId(User);
        string? filePath = null;

        if (UploadedFile != null)
        {
            var uploadDir = Path.Combine("wwwroot", "uploads");  // Ensure this directory exists
            Directory.CreateDirectory(uploadDir); 
            filePath = Path.Combine("uploads", UploadedFile.FileName);

            using (var stream = new FileStream(Path.Combine("wwwroot", filePath), FileMode.Create))
            {
                await UploadedFile.CopyToAsync(stream);
            }
        }

        var document = new Document
        {
            DocumentName = DocumentName,
            FilePath = filePath,
            Status = "In Review",
            UserId = userId,
            UploadDate = DateTime.Now
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    // Delete action
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var document = await _context.Documents.FindAsync(id);

        if (document == null)
        {
            return NotFound();
        }

        // Only allow deletion by uploader or admin
        var userId = _userManager.GetUserId(User);
        if (document.UserId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        // Delete the file from the server
        if (!string.IsNullOrEmpty(document.FilePath))
        {
            var filePath = Path.Combine("wwwroot", document.FilePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        // Remove the document entry from the database
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public string GetBadgeClass(string status)
    {
        return status switch
        {
            "In Review" => "badge-warning",
            "Approved" => "badge-success",
            "Needs Revision" => "badge-danger",
            _ => "badge-secondary",
        };
    }
}


// Relative path: TradeManagementApp\Pages\UserDashboard.cshtml.cs
