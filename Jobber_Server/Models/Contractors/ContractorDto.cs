using System.ComponentModel.DataAnnotations;

namespace Jobber_Server.Models.Contractors
{
    public record ContractorDto(
        int Id,
        string Guid,
        string FirstName,
        string LastName,
        string? BioShort = null,
        string? BioLong = null,
        JobCategory[]? JobCategories = null,
        string[]? Services = null,
        ServiceArea? ServiceArea = null,
        string? ProfilePicture = null,
        string? ProfilePictureThumbnail = null,
        string[]? Portfolio = null,
        string[]? PortfolioThumbnails = null
    );


    public record CreateContractorDto(
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
        JobCategory[]? JobCategories = null,
        string[]? Services = null,
        ServiceArea? ServiceArea = null,
        IFormFile? ProfilePicture = null,
        IFormFile[]? Portfolio = null
    );

    public record UpdateContractorDto(
        [Required(ErrorMessage = "Id is required.")]
        string Id,

        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        string? FirstName,

        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        string? LastName,

        [StringLength(255, ErrorMessage = "Short bio cannot exceed 255 characters.")]
        string? BioShort,

        [StringLength(65535, ErrorMessage = "Long bio cannot exceed 65,535 characters.")]
        string? BioLong,

        JobCategory[]? JobCategories,
        string[]? Services,

        ServiceArea? ServiceArea,
        byte[]? ProfilePicture,
        byte[][]? Portfolio
    );
}