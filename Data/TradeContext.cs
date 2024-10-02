using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TradeManagementApp.Data
{
    // Your DbContext inherits from IdentityDbContext to use ASP.NET Identity
    public class TradeContext : IdentityDbContext<IdentityUser>
    {
        public TradeContext(DbContextOptions<TradeContext> options) : base(options) { }

        // DbSet for Trades and Documents
        public DbSet<TradeData> Trades { get; set; }
        public DbSet<Document> Documents { get; set; }
    }

    // TradeData class to store trade-related info
    public class TradeData
    {
        public int Id { get; set; }
        public string? TradeInfo { get; set; } // Nullable to avoid warnings
    }

    // Document class that stores document upload info
    public class Document
    {
        public int Id { get; set; }  // Primary key
        public string DocumentName { get; set; } = string.Empty; // Ensures no null errors

        // File path for the uploaded document (nullable for cases without uploads)
        public string? FilePath { get; set; }

        // Document status (default to 'In Review')
        public string Status { get; set; } = "In Review";

        // Foreign key linking document to the user who uploaded it
        public string UserId { get; set; } = string.Empty; // Set default to empty string

        // Navigation property to link the document to the IdentityUser
        public IdentityUser? User { get; set; }

        // Upload date to track when the document was uploaded (default to DateTime.Now)
        public DateTime UploadDate { get; set; } = DateTime.Now;

        // New AI score property (0 to 100)
        public float AIScore { get; set; } // Add this for storing AI score
    }
}

// Relative path: TradeManagementApp\Data\TradeContext.cs
