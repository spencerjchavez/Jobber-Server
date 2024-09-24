namespace Jobber_Server.Controllers 
{
    [Route("api/images")]
    [ApiController]
    public class ImagesAPIController(JobberDbContext dbContext): ControllerBase 
    {

        private readonly JobberDbContext _dbContext = dbContext;

        // Get one Contractor
        [HttpGet("{id}")]
        public ActionResult<ImagesDto> UploadImages(UploadImagesDto dto)
        {
            foreach(IFormFile file in dto.images )
            {
                
            }
        }
    }
}