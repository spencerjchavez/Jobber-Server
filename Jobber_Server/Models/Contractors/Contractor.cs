using System.ComponentModel.DataAnnotations;

namespace Jobber_Server.Models.Contractors
{
    public class Contractor
    {
        public required int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? BioShort { get; set; }
        public string? BioLong { get; set; }
        public JobCategory[]? jobCategories { get; set; }
        public string[]? services { get; set; }
        public ServiceArea? ServiceArea { get; set; }
        public string? ProfilePicture { get; set; }
        public string? ProfilePictureThumbnail { get; set; }
        public string[]? Portfolio { get; set; }
        public string[]? PortfolioThumbnails { get; set; }
    }
}