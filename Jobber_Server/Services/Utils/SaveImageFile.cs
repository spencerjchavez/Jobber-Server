
using Jobber_Server.Assets;
using System.Drawing;
namespace Jobber_Server.Services.Utils
{
    class SaveImageFile {
        
        public Uri? ImageUri { get; }
        public Uri? ImageThumbnailUri { get; }
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
                var guid = Guid.NewGuid();
                string fileName = $"{guid}{fileExtension}";
                string filePath = System.IO.Path.Combine("Assets/Images/", fileName);

                // Save the file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    ImageUri = new UriBuilder("http", Constants.HOST, Constants.PORT, filePath).Uri;
                }

                // TODO: Create and save thumbnail image ()
                /*
                var thumbnailFileName = $"{guid}_thumbnail{fileExtension}";
                var thumbnailFilePath = System.IO.Path.Combine("Assets/Images/", fileName);
                using(var stream = file.OpenReadStream())
                {
                    var image = Image.FromStream(stream);

                }
            
                using (var stream = new FileStream(thumbnailFilePath, FileMode.Create))
                {
                    thumbnailFile.CopyTo(stream);
                    ImageUri = new UriBuilder("http", Constants.HOST, Constants.PORT, thumbnailFilePath).Uri; 
                }
                */
                ImageThumbnailUri = ImageUri;
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