
using Jobber_Server.Assets;

namespace Jobber_Server.Models.Images
{
    public record ImagesDto
    {
        private ICollection<string> _imageThumbnails = new List<string>();
        public ICollection<string> ImageThumbnails 
        {
            get => _imageThumbnails; 
            init {
                ValidateUris(value);
                _imageThumbnails = value;
            } 
        }
        private ICollection<string> _images = new List<string>();
        public ICollection<string> Images
        {
            get => _images; 
            init {
                ValidateUris(value);
                _images = value;
            } 
        }

        private static void ValidateUris(ICollection<string> value)
        {
            foreach(string uriString in value) 
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
}