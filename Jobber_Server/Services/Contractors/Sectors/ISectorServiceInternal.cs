using Jobber_Server.Models.Contractors;

namespace Jobber_Server.Services.Contractors.Sectors
{
    public interface ISectorServiceInternal
    {
        void AddContractor(Contractor contractor);
        ICollection<Contractor> GetContractors(double latitude, double longitude);

    }
}