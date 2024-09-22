using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TradeManagementApp.Data;
using System.Threading.Tasks;
using System.IO;

namespace TradeManagementApp.Pages
{
    public class UploadDocumentModel : PageModel
    {
        private readonly TradeContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UploadDocumentModel(TradeContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public string DocumentName { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? UploadedFile { get; set; } // Nullable for optional file upload

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(DocumentName))
            {
                ModelState.AddModelError("DocumentName", "Document Name is required.");
                return Page();
            }

            var userId = _userManager.GetUserId(User);
            string? filePath = null;

            // Only process if a file is uploaded
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

            // Create and save document entry with or without a file
            var document = new Document
            {
                DocumentName = DocumentName,
                FilePath = filePath, // Can be null if no file uploaded
                Status = "In Review",
                UserId = userId
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}

// TradeManagementApp\Pages\UploadDocument.cshtml.cs