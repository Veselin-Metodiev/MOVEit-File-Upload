using Microsoft.AspNetCore.Mvc;

namespace MOVEitFileUpload.Controllers
{
    public class FileUpload : Controller
    {
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Upload(FileUploadViewModel fileUploadViewModel)
        //{

        //}
    }
}
