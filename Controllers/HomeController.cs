using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AscendingFileCreationDate.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;

namespace AscendingFileCreationDate.Controllers
{
    public class HomeController : Controller
    {
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
            
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var formFile in orderedFiles)
                    {
                        var demoFile = archive.CreateEntry(formFile.FileName);
                        
                        using (var entryStream = demoFile.Open())
                        {
                            await formFile.CopyToAsync(entryStream);
                        }
                    }
                    
                }
                return File(memoryStream.ToArray(), "application/zip", "result.zip");
            }
        }
    }
}
