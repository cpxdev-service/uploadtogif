using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace GIFConverterOnline.Controllers
{
    public class UploadController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        public IActionResult clear()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "blob"));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            return Ok();
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000_000)]
        [ValidateAntiForgeryToken] // Good practice for security against CSRF attacks
        public async Task<IActionResult> UploadImages() // Parameter name matches input name="imageFiles"
        {
            try
            {
                var files = Request.Form.Files;
                if (files == null || files.Count == 0)
                {
                    ViewBag.Error = "No files were selected.";
                    return View(); // Return to the view with an error message
                }

                using var collection = new MagickImageCollection();
                foreach (var file in files)
                {
                    await using var stream = file.OpenReadStream();
                    var img = new MagickImage(stream);
                    img.Extent(img.Width, img.Height, Gravity.Forget, MagickColors.Transparent);
                    collection.Add(img);
                }

                // Frame configuration
                foreach (var frame in collection)
                {
                    frame.AnimationDelay = 100;
                    frame.GifDisposeMethod = GifDisposeMethod.Previous;
                }

                collection.Quantize(new QuantizeSettings { Colors = 256 });

                string guid = Guid.NewGuid().ToString();
                var output = Path.Combine(
                    Directory.GetCurrentDirectory(), "blob", $"{guid}.gif");
                collection.Write(output, MagickFormat.Gif);

                return new JsonResult(new { uploadId = "/download/file/" + guid });
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while processing the images: " + ex.Message;
                return new JsonResult(new { msg = ex.Message});
            }
        }
    }
}
