using System.ComponentModel.DataAnnotations;
using Jobber_Server.Models.Images;

namespace Jobber_Server.Models.Contractors
{
    public record ContractorDto(
        int Id,
        string Guid,
        string FirstName,
        string LastName,
        string? BioShort = null,
        string? BioLong = null,
        ICollection<JobCategory>? JobCategories = null,
        ICollection<string>? Services = null,
        ServiceArea? ServiceArea = null,
        ImagesDto? ProfilePicture = null,
        ICollection<ImagesDto>? Portfolio = null
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

        [StringLength(65535, ErrorMessage = "Long bio cannot exceed 65,535 characters.")]
        string? BioLong = null,
        ICollection<int>? JobCategoryIds = null,
        ICollection<string>? Services = null,
        ServiceArea? ServiceArea = null,
        ImagesDto? ProfilePicture = null,
        ICollection<ImagesDto>? Portfolio = null
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

        [StringLength(65535, ErrorMessage = "Long bio cannot exceed 65,535 characters.")]
        string? BioLong,

        ICollection<int>? JobCategoryIds,
        ICollection<string>? Services,

        ServiceArea? ServiceArea,
        ImagesDto? ProfilePicture,
        ICollection<ImagesDto>? Portfolio
    );
}