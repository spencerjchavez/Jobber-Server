using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jobber_Server.Models.Images;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Jobber_Server.Models.Contractors
{
    public record Contractor
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? BioShort { get; set; }
        public string? BioLong { get; set; }
        public virtual ICollection<ContractorJobCategory> ContractorJobCategories { get; set; } = new List<ContractorJobCategory>();
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
        public ImagesDto? ProfilePicture { get; set; }
        [NotMapped]
        public ICollection<ImagesDto>? Portfolio { get; set; }
        [Column("Portfolio")]
        public string? PortfolioJson
        {
            get => JsonSerializer.Serialize(Portfolio);
            set => Portfolio = value == null ? null : JsonSerializer.Deserialize<ICollection<ImagesDto>>(value);
        }
    }
}