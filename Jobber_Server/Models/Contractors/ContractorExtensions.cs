namespace Jobber_Server.Models.Contractors
{
    public static class ContractorExtensions
    {
        public static ContractorDto ToDto(this Contractor contractor)
        {
            return new ContractorDto(
                Id: contractor.Id,
                FirstName: contractor.FirstName,
                LastName: contractor.LastName,
                BioShort: contractor.BioShort,
                BioLong: contractor.BioLong,
                JobCategories: contractor.jobCategories,
                Services: contractor.services,
                ServiceArea: contractor.ServiceArea,
                ProfilePicture: contractor.ProfilePicture,
                ProfilePictureThumbnail: contractor.ProfilePictureThumbnail,
                Portfolio: contractor.Portfolio,
                PortfolioThumbnails: contractor.PortfolioThumbnails
            );
        }
    }
}