using Jobber_Server.DBContext;
using Jobber_Server.Models;
using Jobber_Server.Models.Contractors;
using Jobber_Server.Models.Contractors.Sector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace Jobber_Server.Services.Contractors.Sectors
{
    public class SectorServiceInternal(JobberDbContext dbContext): ISectorServiceInternal
    {

        private readonly JobberDbContext _dbContext = dbContext;

        public void AddContractor(Contractor contractor)
        {
            if(contractor.ServiceArea == null)
            {
                return;
            }
            var contractorLat = contractor.ServiceArea.Latitude;
            var contractorLon = contractor.ServiceArea.Longitude;
            var contractorRadius = contractor.ServiceArea.Radius;

            ISet<int> addContractorToSector(int sectorId, bool addToAllChildren = false)
            {
                var sector = _dbContext.Sectors.Where(s => s.Id == sectorId).FirstOrDefault() ?? throw new Exception($"Couldn't find sector of id {sectorId}");
                if(!addToAllChildren)
                {
                    if(!sector.IntersectsServiceArea(contractor.ServiceArea))
                    {
                        return new HashSet<int>();
                    }
                    addToAllChildren = 
                        contractor.ServiceArea.Contains(sector.LatitudeNorth(), sector.LongitudeWest()) &&
                        contractor.ServiceArea.Contains(sector.LatitudeNorth(), sector.LongitudeEast()) &&
                        contractor.ServiceArea.Contains(sector.LatitudeSouth(), sector.LongitudeEast()) &&
                        contractor.ServiceArea.Contains(sector.LatitudeSouth(), sector.LongitudeWest());                            
                }

                if(sector.HasChildren())
                {
                    var sectorIds = new HashSet<int>();
                    sectorIds.UnionWith(addContractorToSector(sector.NE ?? -1, addToAllChildren));
                    sectorIds.UnionWith(addContractorToSector(sector.NW ?? -1, addToAllChildren));
                    sectorIds.UnionWith(addContractorToSector(sector.SW ?? -1, addToAllChildren));
                    sectorIds.UnionWith(addContractorToSector(sector.SE ?? -1, addToAllChildren));
                    return sectorIds;
                } else {
                    _dbContext.ContractorSectors.Add(new ContractorSector{
                        SectorId = sector.Id,
                        ContractorId = contractor.Id
                    });
                    return new HashSet<int>{sector.Id};
                }
            }

            var sectorIds = new HashSet<int>();
            if(contractorLon - contractorRadius / 6378 < 0) 
            {
                //western hemisphere
                var s =_dbContext.Sectors.Where(s => s.Id == 1).FirstOrDefault() ?? throw new Exception("Couldn't get head sector");
                sectorIds.UnionWith(addContractorToSector(s.Id));
            }
            if(contractorLon + contractorRadius / 6378 >= 0)
            {
                // eastern hemisphere
                var s =_dbContext.Sectors.Where(s => s.Id == 2).FirstOrDefault() ?? throw new Exception("Couldn't get head sector");
                sectorIds.UnionWith(addContractorToSector(s.Id));
            }
            _dbContext.SaveChanges();

            var contractorSectors = _dbContext.ContractorSectors.Where(cs => sectorIds.Contains(cs.SectorId));
            var contractorsPerSector = new Dictionary<int, int>();
            foreach(var cs in contractorSectors)
            {
                if(!contractorsPerSector.TryGetValue(cs.SectorId, out int count))
                {
                    count = 0;
                }
                contractorsPerSector[cs.SectorId] = count + 1;
            }
            foreach(KeyValuePair<int, int> kvp in contractorsPerSector)
            {
                if(kvp.Value >= 100) {
                    SplitSector(kvp.Key);
                }
            }
            _dbContext.SaveChanges();
        }

        public ICollection<Contractor> GetContractors(double latitude, double longitude)
        {
            var sector = _dbContext.Sectors.Where(s => s.Id == (longitude < 0 ? 1 : 2)).FirstOrDefault() ?? throw new Exception("Couldn't get head sector");
            
            while (sector.HasChildren())
            {
                var isWest = longitude < sector.Longitude;
                var isSouth = latitude < sector.Latitude;
                var childSectorId = isWest ? (isSouth ? sector.SW : sector.NW) : (isSouth ? sector.SE : sector.NE);
                sector = _dbContext.Sectors.Where(s => s.Id == childSectorId).FirstOrDefault() ?? throw new Exception($"Couldn't find sector of id {childSectorId}");
            }

            var contractorSectors = _dbContext.ContractorSectors
                .Where(cs => cs.SectorId == sector.Id)
                .Include(cs => cs.Contractor)
                .ToList() ?? new List<ContractorSector>();

            return contractorSectors.Select(cs => cs.Contractor).ToList() ?? new List<Contractor>();
        }

        private void SplitSector(int sectorId)
        {
            var sector = _dbContext.Sectors
            .Where(s => s.Id == sectorId)
            .FirstOrDefault() ?? throw new Exception($"Couldn't find sectorId {sectorId}");

            if(sector.Depth > 17) // ensures we don't split a sector whose width < 5km
            {
                return;
            }

            var childCoordinateOffset = sector.GetWidth() / 4;
            var nw = _dbContext.Sectors.Add(new Sector{
                Latitude = sector.Latitude + childCoordinateOffset,
                Longitude = sector.Longitude - childCoordinateOffset,
                Depth = sector.Depth + 1,
                Parent = sectorId 
            }).Entity;
            var ne = _dbContext.Sectors.Add(new Sector{
                Latitude = sector.Latitude + childCoordinateOffset,
                Longitude = sector.Longitude + childCoordinateOffset,
                Depth = sector.Depth + 1,
                Parent = sectorId 
            }).Entity;
            var se = _dbContext.Sectors.Add(new Sector{
                Latitude = sector.Latitude - childCoordinateOffset,
                Longitude = sector.Longitude + childCoordinateOffset,
                Depth = sector.Depth + 1,
                Parent = sectorId 
            }).Entity;
            var sw = _dbContext.Sectors.Add(new Sector{
                Latitude = sector.Latitude - childCoordinateOffset,
                Longitude = sector.Longitude - childCoordinateOffset,
                Depth = sector.Depth + 1,
                Parent = sectorId 
            }).Entity;
            _dbContext.SaveChanges();

            sector.NW = nw.Id;
            sector.NE = ne.Id;
            sector.SE = se.Id;
            sector.SW = sw.Id;

            var contractorSectors = _dbContext.ContractorSectors
            .Where(cs => cs.SectorId == sectorId)
            .Include(cs => cs.Contractor)
            .ToList();

            foreach(var contractorSector in contractorSectors)
            {

                var serviceArea = contractorSector.Contractor.ServiceArea;
                if(serviceArea == null) 
                { 
                    continue; 
                }
                void addContractorSector(Sector childSector)
                {
                    if(sector.IntersectsServiceArea(serviceArea))
                    {
                        _dbContext.ContractorSectors.Add(new ContractorSector
                        {
                            ContractorId = contractorSector.ContractorId,
                            SectorId = childSector.Id
                        });
                    }
                }
                addContractorSector(nw);
                addContractorSector(ne);
                addContractorSector(se);
                addContractorSector(sw);
            }
            _dbContext.SaveChanges();
            _dbContext.ContractorSectors.RemoveRange(contractorSectors);     
            _dbContext.SaveChanges();       
        }
    }
}