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
        public ActionResult<ImagesDto> UploadImages([FromForm] UploadImagesDto dto)
        {
            var toReturn = new ImagesDto();
            foreach(IFormFile file in dto.Images )
            {
                SaveImageFile sif = new SaveImageFile(file);
                if(sif.Error != null) 
                {
                    return BadRequest("Exception: " + sif.Error);
                } else if(sif.ImageUri != null && sif.ImageThumbnailUri != null)
                {
                    toReturn.Images.Add(sif.ImageUri.ToString());
                    toReturn.ImageThumbnails.Add(sif.ImageThumbnailUri.ToString());
                }
            }
            return toReturn;
        }
    }
}