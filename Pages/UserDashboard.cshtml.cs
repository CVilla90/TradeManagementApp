using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TradeManagementApp.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;

public class UserDashboardModel : PageModel
{
    private readonly TradeContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private List<string> Keywords { get; set; }

    public UserDashboardModel(TradeContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;

        // Load the keywords from the JSON file on initialization
        var keywordsFilePath = Path.Combine("wwwroot", "data", "Keywords.json");
        LoadKeywords(keywordsFilePath);
    }

    [BindProperty]
    public string DocumentName { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? UploadedFile { get; set; }

    public List<Document> Documents { get; set; } = new List<Document>();

    // Method to load keywords from the JSON file
    private void LoadKeywords(string jsonFilePath)
    {
        using (var reader = new StreamReader(jsonFilePath))
        {
            var jsonContent = reader.ReadToEnd();
            var jsonDocument = JsonDocument.Parse(jsonContent);
            Keywords = jsonDocument.RootElement.GetProperty("keywords").EnumerateArray()
                        .Select(k => k.GetString().ToLower()).ToList();
        }
    }

    // Method to calculate the AI score based on keyword matching
    private float CalculateAIScore(string documentPath)
    {
        // Read the document text
        var documentText = System.IO.File.ReadAllText(documentPath).ToLower();

        // Count how many keywords match
        int matchCount = Keywords.Count(keyword => documentText.Contains(keyword));

        // Return the AI score as a percentage
        return (float)matchCount / Keywords.Count * 100;
    }

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
            var uploadDir = Path.Combine("wwwroot", "uploads");
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
            UploadDate = DateTime.Now,
            AIScore = CalculateAIScore(Path.Combine("wwwroot", filePath)) // Calculate and assign AI Score
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

    // Add method to get the badge class for the AI score
    public string GetAIScoreClass(float score)
    {
        if (score >= 75) return "badge-success";
        if (score >= 50) return "badge-warning";
        return "badge-danger";
    }
}
