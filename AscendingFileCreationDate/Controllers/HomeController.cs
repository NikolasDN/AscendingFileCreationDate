using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AscendingFileCreationDate.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using WpfControlLibrary1;

namespace AscendingFileCreationDate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        [HttpPost]
        public async Task<IActionResult> Index([FromForm]List<IFormFile> files)
        {
            if (files.Count == 0) throw new Exception("Kaput");
            var orderedFiles = files.OrderBy(o => o.FileName).ToList();
            var startDate = DateTime.Now;

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var formFile in orderedFiles)
                    {
                        await FileMetaData.CreateNewDateTaken(formFile, archive, startDate);
                        startDate = startDate.AddMinutes(1);
                    }

                }
                return File(memoryStream.ToArray(), "application/zip", "result.zip");
            }
        }
    }
}
