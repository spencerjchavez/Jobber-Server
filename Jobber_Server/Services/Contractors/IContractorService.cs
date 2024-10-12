using Jobber_Server.Models;
using Jobber_Server.Models.Contractors;

namespace Jobber_Server.Services.Contractors
{
    public interface IContractorService
    {
        ContractorDto GetContractor(int id);
        ICollection<ContractorDto> GetContractors(double latitude, double longitude, int[] jobCategories);
        int CreateContractor(CreateContractorDto contractor);
        void UpdateContractor(UpdateContractorDto contractor);
        void DeleteContractor(int id);
    }
}