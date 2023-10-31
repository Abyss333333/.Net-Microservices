using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMapper _mapper;
        public PlatformsController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _mapper = mapper;
            _repository = repository;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]   
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("Getting Platforms");

            var platformItem = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        } 

        [HttpGet("{id}", Name = "GetPlatformByID")]   
        public ActionResult<PlatformReadDto> GetPlatformByID([FromRoute] int id)
        {
            Console.WriteLine($"Getting Platform With Id {id} ");

            var platformItem = _repository.GetPlatformById(id);
            if (platformItem == null)
                return BadRequest("Item Not Found With Id");

            return Ok(_mapper.Map<PlatformReadDto>(platformItem));
        } 

        [HttpPost]   
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform([FromBody] PlatformCreateDto request)
        {
            Console.WriteLine($"Creating New Platform with {JsonConvert.SerializeObject(request)} ");

            var platformModel = _mapper.Map<Platform>(request);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            var platformReadDTO = _mapper.Map<PlatformReadDto>(platformModel);
            
            try{
                await _commandDataClient.SendPlatformToCommand(platformReadDTO);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: Cound Not Send Synchronously {ex.Message.ToString()}");
            }

            
            return CreatedAtRoute(nameof(GetPlatformByID), new {Id = platformReadDTO.Id}, platformReadDTO);

        } 
    }
}