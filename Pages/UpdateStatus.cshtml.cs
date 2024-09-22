using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TradeManagementApp.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.IO;

namespace TradeManagementApp.Pages
{
    public class UpdateStatusModel : PageModel
    {
        private readonly TradeContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UpdateStatusModel(TradeContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Document Document { get; set; } = new Document(); // Define Document property

        [BindProperty]
        public string DocumentName { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? UploadedFile { get; set; } // Renamed to avoid conflicts

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(DocumentName))
            {
                ModelState.AddModelError("DocumentName", "Document Name is required.");
                return Page();
            }

            var userId = _userManager.GetUserId(User);  // Get the logged-in user's ID
            string? filePath = null;

            if (UploadedFile != null)
            {
                var uploadDir = Path.Combine("wwwroot", "uploads");
                Directory.CreateDirectory(uploadDir); // Create directory if not exists
                filePath = Path.Combine(uploadDir, UploadedFile.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(stream);
                }
            }

            var document = new Document
            {
                DocumentName = DocumentName,
                FilePath = filePath,
                Status = "In Review",
                UserId = userId,              // Make sure the UserId is saved
                UploadDate = DateTime.Now      // Set the UploadDate
            };

            _context.Documents.Add(document);   // Save document to the database
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

    }
}

// TradeManagementApp\Pages\UpdateStatus.cshtml.cs