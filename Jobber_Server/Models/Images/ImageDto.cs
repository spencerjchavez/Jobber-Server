
using Jobber_Server.Assets;

namespace Jobber_Server.Models.Images
{
    public record ImageDto
    {
        private string _imageThumbnail = "";
        public string ImageThumbnail
        {
            get => _imageThumbnail; 
            init {
                ValidateUri(value);
                _imageThumbnail = value;
            } 
        }
        private string _image = "";
        public string Image
        {
            get => _image; 
            init {
                ValidateUri(value);
                _image = value;
            } 
        }

        private static void ValidateUri(string uriString)
        {
            
            var success = Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri);
            if(!success || uri == null) 
            {
                throw new Exception("Invalid image URL");
            }
            if(uri.Host != Constants.HOST)
            {
                throw new Exception("Invalid image URL");
            }
            
        }
    }
}