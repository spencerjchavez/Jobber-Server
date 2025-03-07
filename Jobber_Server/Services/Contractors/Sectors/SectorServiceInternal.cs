using System.ComponentModel.DataAnnotations;
using Jobber_Server.DBContext;
using Jobber_Server.Models;
using Jobber_Server.Models.Contractors;
using Jobber_Server.Models.Contractors.Sector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
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

            // returns list of contractor sectors to add to database along with a reference to the object's sector
            List<ContractorSector> createContractorSectors(int sectorId, bool servesEntireSector = false)
            { 
                var sector = _dbContext.Sectors.Where(s => s.Id == sectorId).FirstOrDefault() ?? throw new Exception($"Couldn't find sector with id {sectorId}");
                if(!servesEntireSector)
                {
                    if(!contractor.ServiceArea!.ServesSector(sector))
                    {
                        return [];
                    }
                    servesEntireSector = contractor.ServiceArea.ServesEntireSector(sector);                         
                }

                if(sector.HasChildren())
                {
                    var contractorSectors = new List<ContractorSector>();
                    contractorSectors.AddRange(createContractorSectors(sector.NEId ?? -1, servesEntireSector));
                    contractorSectors.AddRange(createContractorSectors(sector.NWId ?? -1, servesEntireSector));
                    contractorSectors.AddRange(createContractorSectors(sector.SWId ?? -1, servesEntireSector));
                    contractorSectors.AddRange(createContractorSectors(sector.SEId ?? -1, servesEntireSector));
                    return contractorSectors;
                } else {
                    var cs = new ContractorSector {
                        SectorId = sector.Id,
                        ContractorId = contractor.Id,
                        ServesEntireSector = servesEntireSector,
                        Sector = sector
                    };
                    return [cs];
                }
            }

            var contractorSectorsToAdd = new List<ContractorSector>();

            if(contractorLon - contractorRadius / 6378 < 0) 
            {
                //western hemisphere
                contractorSectorsToAdd.AddRange(createContractorSectors(1));
            }
            if(contractorLon + contractorRadius / 6378 >= 0)
            {
                // eastern hemisphere
                contractorSectorsToAdd.AddRange(createContractorSectors(2));
            }
            _dbContext.ContractorSectors.AddRange(contractorSectorsToAdd);
            _dbContext.SaveChanges();

            // prepare to split sectors
            var sectorsAddedTo = new List<Sector>(contractorSectorsToAdd.Select(cs => cs.Sector));
            var sectorIdsAddedTo = sectorsAddedTo.Select(s => s.Id);
            var contractorsInSector = _dbContext.ContractorSectors
                .Where(cs => sectorIdsAddedTo.Contains(cs.SectorId))
                .ToList() ?? new List<ContractorSector>();

            var numContractorsNotServingEntireSector = new Dictionary<int, int>();
            var contractorsBySectorId = new Dictionary<int, List<ContractorSector>>();
            var sectorsById = new Dictionary<int, Sector>();
            foreach(var cs in contractorsInSector)
            {
                if(!cs.ServesEntireSector)
                {
                    if(!numContractorsNotServingEntireSector.TryGetValue(cs.Sector.Id, out int count))
                    {
                        count = 0;
                    }
                    numContractorsNotServingEntireSector[cs.Sector.Id] = count + 1;
                }
                sectorsById[cs.Sector.Id] = cs.Sector;
                if(!contractorsBySectorId.TryGetValue(cs.SectorId, out List<ContractorSector>? contractorsInInnerSector))
                {
                    contractorsBySectorId[cs.SectorId] = new List<ContractorSector>();
                }
                contractorsBySectorId[cs.SectorId].Add(cs);
            }
            
            var sectorsToSplit = new List<Sector>();
            var contractorSectorsToSplit = new List<List<ContractorSector>>();
            foreach(KeyValuePair<int, int> kvp in numContractorsNotServingEntireSector)
            {
                if(kvp.Value >= 100) {
                    sectorsToSplit.Add(sectorsById[kvp.Key]);
                    contractorSectorsToSplit.Add(contractorsBySectorId[kvp.Key]);
                }
            }

            SplitSectors(sectorsToSplit, contractorSectorsToSplit);

            _dbContext.SaveChanges();
        }

        // Gets all contractors whose service area intersects a sector that intersects the cirle described by the parameters. 
        // These contractors' service areas may not actually intersect the circle described by the parameters.
        public ICollection<Contractor> GetContractors(double latitude, double longitude, double radius)
        {
            var circle = new ServiceArea
            {
                Latitude=latitude,
                Longitude=longitude,
                Radius=radius
            };

            ICollection<int> GetSectorsInCircleRecursive(Sector sector)
            {
                if (!circle.Intersects(sector)) return [];
                if (!sector.HasChildren()) return [sector.Id];

                var numCornersInCircle = 0;
                if(circle.Contains(sector.LatitudeNorth(), sector.LongitudeWest())) numCornersInCircle++;
                if(circle.Contains(sector.LatitudeNorth(), sector.LongitudeEast())) numCornersInCircle++;
                if(circle.Contains(sector.LatitudeSouth(), sector.LongitudeWest())) numCornersInCircle++;
                if(circle.Contains(sector.LatitudeSouth(), sector.LongitudeEast())) numCornersInCircle++;                
                if (numCornersInCircle == 4) // no need to check bounds of children, just return all leaf nodes
                {
                    var sectorsToDrill = new Stack<Sector>([sector]);
                    var childSectorIdsToReturn = new List<int>();
                    while (sectorsToDrill.Count > 0)
                    {
                        var toDrill = sectorsToDrill.Pop();
                        var children = _dbContext.Sectors
                            .Where(s => s.ParentId == toDrill.Id)
                            .Include(s => s.NE)
                            .Include(s => s.NW)
                            .Include(s => s.SE)
                            .Include(s => s.SW)
                            .ToList() ?? throw new Exception($"Couldn't find children of sector id {toDrill.Id}");
                        children.ForEach((child) => {
                            if (child.HasChildren())
                            {
                                sectorsToDrill.Push(child);
                            } else {
                                childSectorIdsToReturn.Add(child.Id);
                            }
                        });
                    }
                    return childSectorIdsToReturn;
                }
                
                var childSectorIds = new List<int>();
                foreach (var childSector in new List<Sector>([sector.NE!, sector.NW!, sector.SE!, sector.SW!]))
                {
                    childSectorIds.AddRange(GetSectorsInCircleRecursive(_dbContext.Sectors
                    .Where(s => s.Id == childSector.Id)
                    .Include(s => s.NE)
                    .Include(s => s.NW)
                    .Include(s => s.SE)
                    .Include(s => s.SW)
                    .FirstOrDefault() ?? throw new Exception($"Couldn't find sector of id {childSector.Id}")));
                }
                return childSectorIds;
            }


            radius = radius / 6378 * 180 / Math.PI; // change radius to degrees instead of km.
            
            var sectorIdsInCircle = new List<int>();
            if (longitude - radius < 0)
            {
                var sector = _dbContext.Sectors
                    .Where(s => s.Id == 1)
                    .Include(s => s.NE)
                    .Include(s => s.NW)
                    .Include(s => s.SE)
                    .Include(s => s.SW)
                    .FirstOrDefault() ?? throw new Exception("Couldn't get head sector");
                sectorIdsInCircle.AddRange(GetSectorsInCircleRecursive(sector));
            }
            if (longitude + radius > 0)
            {
                var sector = _dbContext.Sectors
                    .Where(s => s.Id == 2)
                    .Include(s => s.NE)
                    .Include(s => s.NW)
                    .Include(s => s.SE)
                    .Include(s => s.SW)
                    .FirstOrDefault() ?? throw new Exception("Couldn't get head sector");
                    sectorIdsInCircle.AddRange(GetSectorsInCircleRecursive(sector));
            }

            var contractorSectors = _dbContext.ContractorSectors
                .Where(cs => sectorIdsInCircle.Contains(cs.SectorId))
                .Include(cs => cs.Contractor)
                .ToList() ?? new List<ContractorSector>();

            return contractorSectors.Select(cs => cs.Contractor).ToList() ?? new List<Contractor>();
        }

        // doesn't save changes to database
        private void SplitSectors(IList<Sector> sectors, List<List<ContractorSector>> associatedContractorSectors)
        {
            var sectorsToAddWithContractorSectors = new List<Sector>();
            for(int i = 0; i < sectors.Count(); i++)
            {
                var sector = sectors[i];
                var contractorSectors = associatedContractorSectors[i];
                
                if(sector.Depth > 9) // ensures we don't split a sector whose width < 20km. This helps limit maximum recursion depths and limit the number of contractors returned from each GET.
                {
                    continue;
                }

                var childCoordinateOffset = sector.Width() / 4;
                var nw = new Sector{
                    Latitude = sector.Latitude + childCoordinateOffset,
                    Longitude = sector.Longitude - childCoordinateOffset,
                    Depth = sector.Depth + 1,
                    Parent = sector
                };
                var ne = new Sector{
                    Latitude = sector.Latitude + childCoordinateOffset,
                    Longitude = sector.Longitude + childCoordinateOffset,
                    Depth = sector.Depth + 1,
                    Parent = sector
                };
                var se = new Sector{
                    Latitude = sector.Latitude - childCoordinateOffset,
                    Longitude = sector.Longitude + childCoordinateOffset,
                    Depth = sector.Depth + 1,
                    Parent = sector
                };
                var sw = new Sector{
                    Latitude = sector.Latitude - childCoordinateOffset,
                    Longitude = sector.Longitude - childCoordinateOffset,
                    Depth = sector.Depth + 1,
                    Parent = sector
                };

                ne.ContractorSectors = splitContractorSectors(contractorSectors, ne);
                nw.ContractorSectors = splitContractorSectors(contractorSectors, nw);
                se.ContractorSectors = splitContractorSectors(contractorSectors, se);
                sw.ContractorSectors = splitContractorSectors(contractorSectors, sw);

                sector.NW = nw;
                sector.NE = ne;
                sector.SE = se;
                sector.SW = sw;
            }

            _dbContext.ContractorSectors.RemoveRange(associatedContractorSectors.SelectMany(cs => cs));    
            return; 

            static List<ContractorSector> splitContractorSectors(List<ContractorSector> contractorSectors, Sector sector)
            {
                var newContractorSectors = new List<ContractorSector>();
                foreach(var contractorSector in contractorSectors)
                {
                    var serviceArea = contractorSector.Contractor.ServiceArea!;
                    var addToSector = false;
                    var servesEntireSector = false;
                    if(contractorSector.ServesEntireSector)
                    {
                        addToSector = true;
                        servesEntireSector = true;
                    } else {
                        if(serviceArea.ServesSector(sector))
                        {
                            addToSector = true;
                            servesEntireSector = serviceArea.ServesEntireSector(sector);
                        }
                    }
                    if(addToSector)
                    {
                        newContractorSectors.Add(new ContractorSector
                        {
                            ContractorId = contractorSector.ContractorId,
                            ServesEntireSector = servesEntireSector
                        });
                    }
                }
                return newContractorSectors;
            }
        }
    }
}