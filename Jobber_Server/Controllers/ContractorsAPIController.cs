using Microsoft.AspNetCore.Mvc;
using Jobber_Server.Models.Contractors;
using Jobber_Server.MicroServices;
using Jobber_Server.DBContext;
using Microsoft.EntityFrameworkCore;
using Jobber_Server.Models;

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
                .FirstOrDefault(contractor => contractor.Id == id);
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
            // TODO: Add Authorization here

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

            var jobCategoryIds = new HashSet<int>();

            var contractorModel = new Contractor
            {
                Guid = contractor.Guid,
                FirstName = contractor.FirstName,
                LastName = contractor.LastName,
                BioShort = contractor.BioShort,
                BioLong = contractor.BioLong,
                ContractorJobCategories = contractor.JobCategoryIds?.Select(jobCategoryId => 
                {
                    if(!jobCategoryIds.Add(jobCategoryId)) // ensure no duplicate jobCategory Ids
                    {
                        throw new Exception("Duplicate job category received");
                    }
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
                Portfolio = contractor.Portfolio,
            };
            _dbContext.Contractors.Add(contractorModel);
            _dbContext.SaveChanges();

            return Ok();
        }

        // Update contractor
        [HttpPut]
        public ActionResult UpdateContractor(UpdateContractorDto contractorUpdated) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var contractor = _dbContext.Contractors
                .Include(contractor => contractor.ContractorJobCategories)
                .FirstOrDefault(contractor => contractor.Id == contractorUpdated.Id);
            if(contractor == null) {
                return NotFound("Contractor Id not found");
            }
            // TODO: add authorization here

            

            if(contractorUpdated.ProfilePicture != null) 
            {
                SaveImageFile sif = new SaveImageFile(contractorUpdated.ProfilePicture);
                if(sif.Error != null) 
                {
                    return BadRequest("Exception: " + sif.Error);
                } else if(sif.Uri != null)
                {
                    contractor.ProfilePicture = sif.Uri.ToString();
                }
            }

            var contractorJobCategoriesToRemove = new HashSet<ContractorJobCategory>(contractor.ContractorJobCategories);
            var contractorJobCategories = contractorUpdated.JobCategoryIds?.Select(jobCategoryId => 
            {
                contractorJobCategoriesToRemove.RemoveWhere(toRemove => toRemove.JobCategoryId == jobCategoryId);
                var jobCategory = _dbContext.JobCategories.Find(jobCategoryId) ?? throw new Exception("Invalid job category received");
                return new ContractorJobCategory
                { 
                    JobCategoryId = jobCategory.Id,
                    JobCategory = jobCategory
                };
            }).ToList() ?? new List<ContractorJobCategory>();

            foreach(ContractorJobCategory contractorJobCategory in contractorJobCategoriesToRemove)
            {
                _dbContext.ContractorJobCategories.Remove(contractorJobCategory);
            }

            contractor.BioShort = contractorUpdated.BioShort;
            contractor.BioLong = contractorUpdated.BioLong;
            contractor.ContractorJobCategories = contractorJobCategories;
            contractor.Portfolio = contractorUpdated.Portfolio;
            contractor.ServiceArea = contractorUpdated.ServiceArea;
            contractor.Services = contractorUpdated.Services;

            _dbContext.SaveChanges();
            return Ok();
        }

        // Delete contractor
        [HttpDelete("{id}")]
        public ActionResult DeleteContractor(int id)
        {
            var contractor = _dbContext.Contractors.FirstOrDefault(contractor => contractor.Id == id);
            if(contractor == null)
            {
                return NotFound("No contractor found with that Id");
            }
            // TODO: add authorization here

            _dbContext.Remove(contractor);
            _dbContext.SaveChanges();
            return Ok();
        }
    }

}