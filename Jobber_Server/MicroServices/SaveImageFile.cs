
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Jobber_Server.MicroServices
{
    class SaveImageFile {
        
        public string Url { get; }
        public string? Error { get; }


        public SaveImageFile(IFormFile file)
        {
            Url = "";
            if (file == null || file.Length == 0)
            {
                Error = "No file provided";
                return;
            }

            try
            {
                // Define the file path and name
                string fileExtension = Path.GetExtension(file.FileName);
                if(fileExtension !=  ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png") {
                    Error = $"File type {fileExtension} is not supported";
                    return;
                }
                string fileName = $"{Guid.NewGuid()}{fileExtension}";
                string filePath = Path.Combine("assets/images/", fileName);

                // Save the file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
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