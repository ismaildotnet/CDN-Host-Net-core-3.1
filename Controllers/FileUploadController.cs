using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace WebApplication1.Controllers
{
	public class FileUploadController : ControllerBase
	{
        private readonly IWebHostEnvironment _environment;

        public FileUploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        [Route("file")]
        public IActionResult UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".docx", ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName);

            if (!allowedExtensions.Contains(fileExtension.ToLower()))
            {
                return BadRequest("Invalid file type.");
            }

            var folderName = "uploads";
            var subfolder = GetSubfolder(fileExtension);

            var webRootPath = _environment.WebRootPath;

            var filePath = Path.Combine(webRootPath, folderName, subfolder, file.FileName);

            var direc = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(direc);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var domainName = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var fileUrl = $"{domainName}/{folderName}/{subfolder}/{file.FileName}";

            return Ok(new { FileUrl = fileUrl });
        }

        private string GetSubfolder(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                    return "images";
                case ".docx":
                    return "docx";
                case ".pdf":
                    return "pdf";
                default:
                    return "other";
            }
        }
    }
}
