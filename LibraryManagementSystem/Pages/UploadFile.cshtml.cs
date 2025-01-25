using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Pages
{
    public class UploadFileModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UploadFileModel> _logger;

        public UploadFileModel(IWebHostEnvironment environment, ILogger<UploadFileModel> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [BindProperty]
        public IFormFile UploadedFile { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate file input
            if (UploadedFile == null)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return Page();
            }

            // Validate file size (e.g., max 5 MB)
            if (UploadedFile.Length > 5 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "File size cannot exceed 5 MB.";
                return Page();
            }

            // Validate file type (only images: jpg, png)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(UploadedFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["ErrorMessage"] = "Invalid file type. Only JPG and PNG files are allowed.";
                return Page();
            }

            // Save the file to the "uploads" folder in wwwroot
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await UploadedFile.CopyToAsync(fileStream);
            }

            _logger.LogInformation("File uploaded successfully: {FilePath}", filePath);
            TempData["SuccessMessage"] = $"File uploaded successfully. File path: /uploads/{uniqueFileName}";

            return Page();
        }
    }
}
