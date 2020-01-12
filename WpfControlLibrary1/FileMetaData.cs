using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfControlLibrary1
{
    public class FileMetaData
    {
        public static async Task CreateNewDateTaken(IFormFile formFile, ZipArchive archive, DateTime dateTaken)
        {
            using (var fs = formFile.OpenReadStream())
            {
                BitmapDecoder decoder2 = BitmapDecoder.Create(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

                FileStream stream3 = new FileStream(formFile.FileName, FileMode.Create);
                BitmapMetadata myBitmapMetadata = new BitmapMetadata("jpg");
                JpegBitmapEncoder encoder3 = new JpegBitmapEncoder();
                myBitmapMetadata.DateTaken = dateTaken.ToString(); // "5/23/2010";

                // Create a new frame that is identical to the one 
                // from the original image, except for the new metadata. 
                encoder3.Frames.Add(
                    BitmapFrame.Create(
                    decoder2.Frames[0],
                    decoder2.Frames[0].Thumbnail,
                    myBitmapMetadata,
                    decoder2.Frames[0].ColorContexts));

                encoder3.Save(stream3);

                var demoFile = archive.CreateEntry(formFile.FileName);

                using (var entryStream = demoFile.Open())
                {
                    stream3.Position = 0;
                    //await formFile.CopyToAsync(entryStream);
                    await stream3.CopyToAsync(entryStream);
                }

                stream3.Close();

                //return stream3;
            }
        }
    }
}
