using Microsoft.AspNetCore.Mvc;
using Jobber_Server.Models.Contractors;
using Jobber_Server.MicroServices;
using Jobber_Server.DBContext;
using Microsoft.EntityFrameworkCore;
using Jobber_Server.Models;
using Jobber_Server.Models.Images;

namespace Jobber_Server.Controllers 
{
    [Route("api/images")]
    [ApiController]
    public class ImagesAPIController(): ControllerBase 
    {
        [HttpPost]
        public ActionResult<ImagesDto> UploadImages(UploadImagesDto dto)
        {
            var toReturn = new ImagesDto();
            foreach(IFormFile file in dto.images )
            {
                SaveImageFile sif = new SaveImageFile(file);
                if(sif.Error != null) 
                {
                    return BadRequest("Exception: " + sif.Error);
                } else if(sif.Uri != null)
                {
                    toReturn.Images.Add(sif.Uri.ToString());
                    toReturn.ImageThumbnails.Add(sif.Uri.ToString());
                }
            }
            return toReturn;
        }
    }
}