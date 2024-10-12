using Microsoft.AspNetCore.Mvc;
using Jobber_Server.Models.Contractors;
using Jobber_Server.Services.Contractors;
using Jobber_Server.Models;
using Org.BouncyCastle.Asn1.Cms;

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
            var start = DateTime.Now;
            for(var i = 0; i < 10000; i++)
            {
                CreateContractor(new CreateContractorDto(
                    new Guid(),
                    "Test" + i,
                    "Test" + i,
                    ServiceArea: new ServiceArea {
                        //Latitude = (180 * random.NextDouble()) - 90,
                        //Longitude = (360 * random.NextDouble()) - 180,
                        Latitude = 0 + random.NextDouble(),
                        Longitude = -90 + random.NextDouble(),
                        Radius = 2.5,
                    }
                ));
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