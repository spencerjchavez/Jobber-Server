using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jobber_Server.Models.Contractors.ContactInfos;
using Jobber_Server.Models.Contractors.OperatingHours;
using Jobber_Server.Models.Contractors.Sector;
using Jobber_Server.Models.Images;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Jobber_Server.Models.Contractors
{
    public class Contractor
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName {get; set; } = string.Empty;
        public string? BioShort { get; set; }
        [NotMapped]
        public ICollection<ContactInfo>? ContactInfos { get; set; }
        [Column("ContactInfo")]
        public string? ContactInfosJson
        {
            get =>  JsonSerializer.Serialize(ContactInfos);
            set => ContactInfos = value == null ? null : JsonSerializer.Deserialize<ICollection<ContactInfo>>(value);
        } 
        [NotMapped]
        public OperatingHoursWeek? OperatingHours { get; set; }
        [Column("OperatingHours")]
        public string? OperatingHoursJson
        {
            get =>  JsonSerializer.Serialize(OperatingHours);
            set => OperatingHours = value == null ? null : JsonSerializer.Deserialize<OperatingHoursWeek>(value);
        } 
        public virtual ICollection<ContractorJobCategory> ContractorJobCategories { get; set; } = new HashSet<ContractorJobCategory>();
        [NotMapped]
        public ICollection<string>? Services { get; set; }

        [Column("Services")]
        public string? ServicesJson
        {
            get => JsonSerializer.Serialize(Services);
            set => Services = value == null ? null : JsonSerializer.Deserialize<string[]>(value);
        }
        [NotMapped]
        public ServiceArea? ServiceArea { get; set; }

        [Column("ServiceArea")]
        public string? ServiceAreaJson
        {
            get =>  JsonSerializer.Serialize(ServiceArea);
            set => ServiceArea = value == null ? null : JsonSerializer.Deserialize<ServiceArea>(value);
        }

        // TODO: May need to look into storing the ImageDto Id within the Contractor object and using a navigation property to pull in the images. 
        [NotMapped]
        public ImageDto? ProfilePicture { get; set; }
        [Column("ProfilePicture")]
        public string? ProfilePictureJson
        {
            get => JsonSerializer.Serialize(ProfilePicture);
            set => ProfilePicture = value == null ? null : JsonSerializer.Deserialize<ImageDto>(value);
        }
        [NotMapped]
        public ICollection<ImageDto>? Portfolio { get; set; }
        [Column("Portfolio")]
        public string? PortfolioJson
        {
            get => JsonSerializer.Serialize(Portfolio);
            set => Portfolio = value == null ? null : JsonSerializer.Deserialize<ICollection<ImageDto>>(value);
        }

        public virtual ICollection<ContractorSector> ContractorSectors { get; set; } = new HashSet<ContractorSector>();
    }
}