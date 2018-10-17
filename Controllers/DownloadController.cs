using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace MMX4.WebAPI.Controllers
{
    public class DownloadController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public DownloadController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Route("api/[controller]")]
        public FileResult downloadFile(string fileName)
        {
            string folderName = "Upload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            var fullPath = Path.Combine(newPath, fileName);
            var stream = new FileStream(fullPath, FileMode.Open);
            var mimeType = "application/x-msdownload";
            return File(stream, mimeType, fileName);
        }
    }
}