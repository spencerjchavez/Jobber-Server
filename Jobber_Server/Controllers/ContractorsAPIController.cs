using Microsoft.AspNetCore.Mvc;
using Jobber_Server.Models.Contractors;
using Jobber_Server.MicroServices;
using System.Runtime.CompilerServices;
using Jobber_Server.DBContext;
using Microsoft.EntityFrameworkCore;
using Jobber_Server.Models;
using System.Collections.ObjectModel;

namespace Jobber_Server.Controllers 
{
    [Route("api/contractors")]
    [ApiController]
    public class ContractorsAPIController(JobberDbContext dbContext): ControllerBase 
    {

        private readonly JobberDbContext _dbContext = dbContext;

        // Get one Contractor
        [HttpGet("{id}")]
        public ActionResult<ContractorDto> GetContractor(int id)
        {
            var contractor = _dbContext.Contractors
                .Include(contractor => contractor.ContractorJobCategories)
                .ThenInclude(contractorJobCategories => contractorJobCategories.JobCategory)
                .Where(contractor => contractor.Id == id).FirstOrDefault();
            if(contractor == null)
            {
                return NotFound("No contractor found");
            } else 
            {
                return Ok(contractor.ToDto());
            }
        }

        // Get many Contractors by location and other filters
        // TODO: put limits on radius and implement pagination of results to ease server resources
        [HttpPost("page")]
        public ActionResult<ICollection<ContractorDto>> GetContractors(int page, double latitude, double longitude, double radius, [FromBody] int[] jobTypes)
        {
            return Ok(new List<ContractorDto> {});
        }

        // Create new contractor
        [HttpPost]
        public ActionResult CreateContractor(CreateContractorDto contractor) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Uri? profileUri = null;
            if(contractor.ProfilePicture != null) 
            {
                SaveImageFile sif = new SaveImageFile(contractor.ProfilePicture);
                if(sif.Error != null) 
                {
                    return BadRequest("Exception: " + sif.Error);
                } else if(sif.Uri != null)
                {
                    profileUri = sif.Uri;
                }
            }

            List<Uri> portfolioUris = [];
            if(contractor.Portfolio != null) 
            {
                foreach (IFormFile file in contractor.Portfolio) 
                {
                    SaveImageFile sif = new SaveImageFile(file);
                    if(sif.Error != null) 
                    {
                        return BadRequest("Exception: " + sif.Error);
                    } else if(sif.Uri != null) 
                    {
                        portfolioUris.Add(sif.Uri);
                    }
                }
            }

            var contractorModel = new Contractor
            {
                Guid = contractor.Guid,
                FirstName = contractor.FirstName,
                LastName = contractor.LastName,
                BioShort = contractor.BioShort,
                BioLong = contractor.BioLong,
                ContractorJobCategories = contractor.JobCategoryIds?.Select(jobCategoryId => 
                {
                    var jobCategory = _dbContext.JobCategories.Find(jobCategoryId) ?? throw new Exception("Invalid job category received");
                    return new ContractorJobCategory
                    { 
                        JobCategoryId = jobCategory.Id,
                        JobCategory = jobCategory
                    };
                }).ToList() ?? new List<ContractorJobCategory>(),
                Services = contractor.Services,
                ServiceArea = contractor.ServiceArea,
                ProfilePicture = profileUri?.ToString(),
                ProfilePictureThumbnail = profileUri?.ToString(),
                Portfolio = portfolioUris.Select(uri => uri.ToString()).ToList(),
                PortfolioThumbnails = portfolioUris.Select(uri => uri.ToString()).ToList(),
            };
            _dbContext.Contractors.Add(contractorModel);
            _dbContext.SaveChanges();

            return Ok();
        }

        // Update contractor


        // Delete contractor


    }
}