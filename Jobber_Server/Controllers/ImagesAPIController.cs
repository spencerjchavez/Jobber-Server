using Microsoft.AspNetCore.Mvc;
using Jobber_Server.Services.Utils;
using Jobber_Server.Models.Images;

namespace Jobber_Server.Controllers 
{
    [Route("api/images")]
    [ApiController]
    public class ImagesAPIController(): ControllerBase 
    {
        [HttpPost]
        public ActionResult<ICollection<ImageDto>> UploadImages([FromForm] UploadImagesDto dto)
        {
            var toReturn = new List<ImageDto>();
            foreach(IFormFile file in dto.Images )
            {
                SaveImageFile sif = new SaveImageFile(file);
                if(sif.Error != null) 
                {
                    return BadRequest("Exception: " + sif.Error);
                } else if(sif.ImageUri != null && sif.ImageThumbnailUri != null)
                {
                    toReturn.Add(new ImageDto{
                        Image = sif.ImageUri.ToString(),
                        ImageThumbnail = sif.ImageThumbnailUri.ToString()
                    });
                }
            }
            return toReturn;
        }
    }
}