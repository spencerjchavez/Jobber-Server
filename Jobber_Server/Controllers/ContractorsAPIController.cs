using Microsoft.AspNetCore.Mvc;
using Jobber_Server.Models.Contractors;
using Jobber_Server.Services.Contractors;
using Jobber_Server.Models;
using Jobber_Server.Models.Images;

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
            var exampleLogos = new List<string>{"http://127.0.0.1:5253/Assets/Images/fd249b25-aa47-40ee-bbec-6fbaf2363dee.png", "http://127.0.0.1:5253/Assets/Images/6fce3708-35ba-401a-9c95-219d3e49fd81.jpg", "http://127.0.0.1:5253/Assets/Images/6b4df110-c476-4476-a700-a5929ff0fa87.jpg", "http://127.0.0.1:5253/Assets/Images/14e8e666-adf5-4041-b569-cbd100e2f27a.jpg"};
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
                        Guid.NewGuid(),
                        "Test" + i,
                        "Test" + i,
                        ServiceArea: new ServiceArea {
                            Latitude = 40.0938 + 2 * random.NextDouble(),
                            Longitude = -112.4585 + 2 * random.NextDouble(),
                            Radius = 10 + 50 * random.NextDouble(),
                        },
                        ProfilePicture: new ImageDto{
                            Image = exampleLogos[(int) (random.NextDouble() * exampleLogos.Count)],
                            ImageThumbnail = exampleLogos[(int) (random.NextDouble() * exampleLogos.Count)],
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
        public ActionResult<ICollection<ContractorDto>> GetContractors(double latitude, double longitude, [FromBody] int[]? jobCategories=null, int page=0)
        {
            return Ok(_service.GetContractors(latitude, longitude, jobCategories ?? []));
        }

        // TODO: Add GetContractors() endpoint to get all contractors whose location is within a circle. Params will be latitude, longitude, radius, jobCategories, pages.

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