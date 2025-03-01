using System.ComponentModel.DataAnnotations;
using Jobber_Server.Models.Contractors.ContactInfos;
using Jobber_Server.Models.Contractors.OperatingHours;
using Jobber_Server.Models.Images;

namespace Jobber_Server.Models.Contractors
{
    public record ContractorDto(
        int Id,
        string Guid,
        string FirstName,
        string LastName,
        string? BioShort = null,
        ICollection<ContactInfo>? ContactInfos = null,
        OperatingHoursWeek? OperatingHours = null,
        ICollection<JobCategory>? JobCategories = null,
        ICollection<string>? Services = null,
        ServiceArea? ServiceArea = null,
        ImageDto? ProfilePicture = null,
        ICollection<ImageDto>? Portfolio = null
    );

    public record CreateContractorDto(
        [Required]
        Guid Guid,

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        string FirstName,

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        string LastName,

        [StringLength(255, ErrorMessage = "Short bio cannot exceed 255 characters.")]
        string? BioShort = null,

        ICollection<ContactInfo>? ContactInfos = null,
        OperatingHoursWeek? OperatingHours = null,
        ICollection<int>? JobCategoryIds = null,
        ICollection<string>? Services = null,
        ServiceArea? ServiceArea = null,
        ImageDto? ProfilePicture = null,
        ICollection<ImageDto>? Portfolio = null
    );

    public record UpdateContractorDto(
        [Required]
        Guid Guid,

        [Required(ErrorMessage = "Id is required.")]
        int Id,

        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        string? FirstName,

        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        string? LastName,

        [StringLength(255, ErrorMessage = "Short bio cannot exceed 255 characters.")]
        string? BioShort,

        ICollection<ContactInfo>? ContactInfos = null,
        OperatingHoursWeek? OperatingHours = null,

        ICollection<int>? JobCategoryIds = null,
        ICollection<string>? Services = null,

        ServiceArea? ServiceArea = null,
        ImageDto? ProfilePicture = null,
        ICollection<ImageDto>? Portfolio = null
    );
}