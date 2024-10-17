using Microsoft.AspNetCore.Mvc;
using Jobber_Server.Models.Contractors;
using Jobber_Server.Services.Contractors;
using Jobber_Server.Models;

namespace Jobber_Server.Controllers 
{
    [Route("api/contractors")]
    [ApiController]
    public class ContractorsAPIController(IContractorService service): ControllerBase 
    {
        
        private readonly IContractorService _service = service;


        [HttpPost("test")]
        public ActionResult Test()
        {
            var random = new Random(0);
            Console.WriteLine("Starting Test");
            var chunkTimes = new List<double>();
            var chunkSize = 200;
            var start = DateTime.Now;
            for(var chunk = 0; chunk < 100; chunk++)
            {
                var chunkStart = DateTime.Now;
                for(var i = 0; i < chunkSize; i++)
                {
                    CreateContractor(new CreateContractorDto(
                        new Guid(),
                        "Test" + i,
                        "Test" + i,
                        ServiceArea: new ServiceArea {
                            Latitude = 20 + random.NextDouble(),
                            Longitude = 20 + random.NextDouble(),
                            //Latitude = 0 + random.NextDouble(),
                            //Longitude = -90 + random.NextDouble(),
                            Radius = 10 + 50 * random.NextDouble(),
                        }
                    ));
                }
                chunkTimes.Add(DateTime.Now.Subtract(chunkStart).TotalSeconds);
            }
            string filePath = System.IO.Path.Combine("_Misc/", "metrics.csv");
            using (var stream = new StreamWriter(filePath))
            {
                stream.WriteLine("Contractors Added, Time Taken");
                var contractorsAdded = chunkSize;
                foreach(var chunkTime in chunkTimes)
                {
                    stream.WriteLine(contractorsAdded + "," + chunkTime);
                    contractorsAdded += chunkSize;
                }
            }
            Console.WriteLine($"Finished in {DateTime.Now.Subtract(start).TotalSeconds} seconds");

            return Ok();
        }
        
        [HttpPost]
        public ActionResult CreateContractor(CreateContractorDto contractor) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var res = new {id = _service.CreateContractor(contractor)};
            return Ok(res);
        }
        
        [HttpGet("{id}")]
        public ActionResult<ContractorDto> GetContractor(int id)
        {
            return Ok(_service.GetContractor(id));
        }

        [HttpPost("page/{page?}")]
        public ActionResult<ICollection<ContractorDto>> GetContractors(double latitude, double longitude, [FromBody] int[] jobCategories, int page=0)
        {
            return Ok(_service.GetContractors(latitude, longitude, jobCategories));
        }

        [HttpPut]
        public ActionResult UpdateContractor(UpdateContractorDto contractorUpdated) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _service.UpdateContractor(contractorUpdated);
            return Ok();
        }

        // Delete contractor
        [HttpDelete("{id}")]
        public ActionResult DeleteContractor(int id)
        {
            _service.DeleteContractor(id);
            return Ok();
        }
    }
}