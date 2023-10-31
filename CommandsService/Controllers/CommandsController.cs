using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Create Command For Platform with given Id
        /// </summary>
        /// <param name="platformId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform( [FromRoute] int platformId)
        {
            Console.WriteLine($"--> GetCommandsForPlatform: {platformId}");

            if (!_repository.PlaformExits(platformId))
            {
                return NotFound();
            }

            var commands = _repository.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        /// <summary>
        /// Get All Commands For Platform with given Id
        /// </summary>
        /// <param name="platformId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform( [FromRoute] int platformId, [FromRoute] int commandId)
        {
            Console.WriteLine($"--> GetCommandForPlatform: {platformId} / {commandId}");

            if (!_repository.PlaformExits(platformId))
            {
                return NotFound();
            }

            var command = _repository.GetCommand(platformId, commandId);

            if(command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        /// <summary>
        /// Create Commands For Platform With Given Id
        /// </summary>
        /// <param name="platformId"></param>
        /// <param name="commandDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform([FromRoute] int platformId, CommandCreateDto commandDto)
        {
             Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

            if (!_repository.PlaformExits(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandDto);

            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new {platformId = platformId, commandId = commandReadDto.Id}, commandReadDto);
        }

    }
}