using Microsoft.AspNetCore.Mvc;
using Jobber_Server.Models.Contractors;
using Jobber_Server.MicroServices;

namespace Jobber_Server.Controllers 
{
    [Route("api/contractors")]
    [ApiController]
    public class ContractorsAPIController: ControllerBase 
    {
        // Get one Contractor
        [HttpGet("{id}")]
        public ActionResult<ContractorDto> GetContractor(int id)
        {
            return Ok(new ContractorDto(Id: 0, FirstName: "Dallen", LastName: "Mathias"));
        }

        // Get many Contractors by location and other filters
        // TODO: put limits on radius and implement pagination of results to ease server resources
        [HttpPost("page")]
        public ActionResult<IEnumerable<ContractorDto>> GetContractors(int page, double latitude, double longitude, double radius, [FromBody] int[] jobTypes)
        {
            return Ok(new List<ContractorDto> {});
        }

        // Create new contractor
        [HttpPost]
        public ActionResult CreateContractor([FromForm] CreateContractorDto contractor) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(contractor.ProfilePicture != null) {
                SaveImageFile sif = new SaveImageFile(contractor.ProfilePicture);
                if(sif.Error != null) {
                    return BadRequest("Exception: " + sif.Error);
                }       
            }
            if(contractor.Portfolio != null) {
                foreach (IFormFile file in contractor.Portfolio) {
                    SaveImageFile sif = new SaveImageFile(file);
                    if(sif.Error != null) {
                        return BadRequest("Exception: " + sif.Error);
                    }
                }
            }
            return Ok();
        }

        // Update contractor


        // Delete contractor


    }
}