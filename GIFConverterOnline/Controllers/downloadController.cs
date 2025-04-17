using Microsoft.AspNetCore.Mvc;

namespace GIFConverterOnline.Controllers
{
    public class downloadController : Controller
    {
        [HttpGet]
        public IActionResult file([FromRoute] string id)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "blob", $"{id}.gif");
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }
            byte[] data = System.IO.File.ReadAllBytes(filePath);
         
            System.IO.File.Delete(filePath);
            var contentType = "image/gif";
            return File(data, contentType, $"{id}.gif");
        }
    }
}
