using Microsoft.AspNetCore.Mvc;
using MOVEitFileUpload.Services.Interfaces;
using MOVEitFileUpload.ViewModels;

using static MOVEitFileUpload.Common.Messages;

namespace MOVEitFileUpload.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IFileUploadService fileUploadService;

        public FileUploadController(IFileUploadService fileUploadService)
        {
            this.fileUploadService = fileUploadService;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(FileUploadViewModel fileUploadViewModel)
        {
            try
            {
                fileUploadService.ValidateInput(fileUploadViewModel);

                await fileUploadService.UploadFile(fileUploadViewModel);

                ViewData["SuccessMessage"] = SuccessfulyUploadedFile;
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
            }

            return View();
        }
    }
}
