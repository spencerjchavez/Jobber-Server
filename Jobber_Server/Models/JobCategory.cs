using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Jobber_Server.Models {
    public class JobCategory 
    {
        public required int Id { get; set; }
        public required string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<ContractorJobCategory>? ContractorJobCategories { get; set; }
    }
}