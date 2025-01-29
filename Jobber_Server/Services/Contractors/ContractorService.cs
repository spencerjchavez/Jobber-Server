using Jobber_Server.DBContext;
using Jobber_Server.Exceptions;
using Jobber_Server.Models;
using Jobber_Server.Models.Contractors;
using Jobber_Server.Models.Contractors.Sector;
using Jobber_Server.Services.Contractors.Sectors;
using Jobber_Server.Services.Utils;
using Microsoft.EntityFrameworkCore;

namespace Jobber_Server.Services.Contractors
{
    public class ContractorService(JobberDbContext dbContext, ISectorServiceInternal sectorService) : IContractorService
    {
        private readonly JobberDbContext _dbContext = dbContext;
        private readonly ISectorServiceInternal _sectorService = sectorService;

        public int CreateContractor(CreateContractorDto contractor)
        {
            // TODO: Add Authorization here

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
                }).ToHashSet() ?? new HashSet<ContractorJobCategory>(),
                Services = contractor.Services,
                ServiceArea = contractor.ServiceArea,
                ProfilePicture = contractor.ProfilePicture,
                Portfolio = contractor.Portfolio,
            };
            var contractorEntity = _dbContext.Contractors.Add(contractorModel);
            _dbContext.SaveChanges();
            _sectorService.AddContractor(contractorEntity.Entity);
            _dbContext.SaveChanges();

            return contractorEntity.Entity.Id;
        }

        public ContractorDto GetContractor(int id)
        {
            var contractor = _dbContext.Contractors
                .Include(contractor => contractor.ContractorJobCategories)
                .ThenInclude(contractorJobCategories => contractorJobCategories.JobCategory)
                .FirstOrDefault(contractor => contractor.Id == id) ?? throw new NotFoundException();
            return contractor.ToDto();
        }

        // TODO: put limits on radius of results and implement pagination of results to ease server resources
        // TODO: check jobCategory filter
        public ICollection<ContractorDto> GetContractors(double latitude, double longitude, int[] jobCategories)
        {
            var contractors = _sectorService.GetContractors(latitude, longitude);
            contractors = contractors.Where(contractor => {
                if(contractor.ServiceArea == null) return false;
                return contractor.ServiceArea.Contains(latitude, longitude);
            }).ToList() ?? new List<Contractor>();
            var contractorDtos = contractors.Select(c => c.ToDto()).ToList();
            return contractorDtos;
        }

        public void UpdateContractor(UpdateContractorDto contractorUpdated)
        {
            var contractor = _dbContext.Contractors
                .Include(contractor => contractor.ContractorJobCategories)
                .FirstOrDefault(contractor => contractor.Id == contractorUpdated.Id) ?? throw new NotFoundException();
            
            // TODO: add authorization here

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
            }).ToHashSet() ?? new HashSet<ContractorJobCategory>();

            foreach(ContractorJobCategory contractorJobCategory in contractorJobCategoriesToRemove)
            {
                _dbContext.ContractorJobCategories.Remove(contractorJobCategory);
            }

            contractor.BioShort = contractorUpdated.BioShort;
            contractor.BioLong = contractorUpdated.BioLong;
            contractor.ContractorJobCategories = contractorJobCategories;
            contractor.ProfilePicture = contractorUpdated.ProfilePicture;
            contractor.Portfolio = contractorUpdated.Portfolio;
            contractor.ServiceArea = contractorUpdated.ServiceArea;
            contractor.Services = contractorUpdated.Services;

            _dbContext.SaveChanges();
        }

        public void DeleteContractor(int id)
        {
            var contractor = _dbContext.Contractors.FirstOrDefault(contractor => contractor.Id == id) 
                ?? throw new NotFoundException("No contractor found with that Id");
            // TODO: add authorization here

            _dbContext.Remove(contractor);
            _dbContext.SaveChanges();   
        }
    }
}