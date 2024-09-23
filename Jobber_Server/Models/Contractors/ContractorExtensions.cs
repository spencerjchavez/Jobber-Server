namespace Jobber_Server.Models.Contractors
{
    public static class ContractorExtensions
    {
        public static ContractorDto ToDto(this Contractor contractor)
        {
            return new ContractorDto
            ( 
                Id: contractor.Id,
                Guid: contractor.Guid.ToString(),
                FirstName: contractor.FirstName,
                LastName: contractor.LastName,
                BioShort: contractor.BioShort,
                BioLong: contractor.BioLong,
                JobCategories: contractor.ContractorJobCategories?.Select(contractorJobCategory => contractorJobCategory.JobCategory).ToList(),
                Services: contractor.Services,
                ServiceArea: contractor.ServiceArea,
                ProfilePicture: contractor.ProfilePicture,
                ProfilePictureThumbnail: contractor.ProfilePictureThumbnail,
                Portfolio: contractor.Portfolio,
                PortfolioThumbnails: contractor.PortfolioThumbnails
            );
        }
    }
}