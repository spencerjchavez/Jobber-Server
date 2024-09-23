
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Jobber_Server.MicroServices
{
    class SaveImageFile {
        
        public Uri? Uri { get; }
        public string? Error { get; }


        public SaveImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                Error = "No file provided";
                return;
            }

            try
            {
                // Define the file path and name
                string fileExtension = System.IO.Path.GetExtension(file.FileName);
                if(fileExtension !=  ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png") {
                    Error = $"File type {fileExtension} is not supported";
                    return;
                }
                string fileName = $"{Guid.NewGuid()}{fileExtension}";
                string filePath = System.IO.Path.Combine("assets/images/", fileName);

                // Save the file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    Uri = new UriBuilder("http", "127.0.0.1", 5253, filePath).Uri; 
                }

                return;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return;
            }
        }
    }
}