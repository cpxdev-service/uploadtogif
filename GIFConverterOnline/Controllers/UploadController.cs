using Microsoft.AspNetCore.Mvc;

namespace GIFConverterOnline.Controllers
{
    public class UploadController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        [ValidateAntiForgeryToken] // Good practice for security against CSRF attacks
        public async Task<IActionResult> UploadImages(List<IFormFile> imageFiles) // Parameter name matches input name="imageFiles"
        {
            if (imageFiles == null || imageFiles.Count == 0)
            {
                ViewBag.Error = "No files were selected.";
                return View(); // Return to the view with an error message
            }

            return View();
        }

    }
}
