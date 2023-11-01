using System.Text.Json;
using CommandsService.Models;
using StackExchange.Redis;

namespace CommandsService.Data
{
    public class CommandRedisRepo : ICommandRepo
    {
        private readonly IConnectionMultiplexer _redis;
        public CommandRedisRepo(IConnectionMultiplexer redis)
        {
            _redis = redis;
            Console.WriteLine("Using Redis Repo");
        }
        public void CreatePlatform(Platform plat)
        {
             if (plat == null)
            {
                throw new ArgumentOutOfRangeException(nameof(plat));
            }
            try{
                var db = _redis.GetDatabase();

                Random rnd = new Random();
                plat.Id = rnd.Next(1,1000);

                var serialPlat = JsonSerializer.Serialize(plat);
                //db.StringSet(plat.Id, serialPlat);
                
                db.HashSet($"hashplatform", new HashEntry[] 
                    {new HashEntry( "Platform:"+ plat.Id, serialPlat)});
            }
            catch(Exception e)
            {
                Console.WriteLine($"Create Platform Failed due to {e.Message}");
            }
        }

        public bool ExternalPlatformExists(int externalPlatformId)
        {
            var platforms = GetAllPlatforms();
            return platforms.Any(p => p != null && p.ExternalID == externalPlatformId);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            var db = _redis.GetDatabase();

            var completeSet = db.HashGetAll("hashplatform");
            var result = new List<Platform>();
            if (completeSet.Length != 0)
            {
                foreach (var item in completeSet)
                {
                    var desPlat = JsonSerializer.Deserialize<Platform>(item.Value);
                    if (desPlat != null)
                        result.Add(desPlat);
                }
            }

            return result;

        }
        public bool PlaformExits(int platformId)
        {
            var db = _redis.GetDatabase();

            //var plat = db.StringGet(id);

            var plat = db.HashGet("hashplatform", "Platform:"+ platformId);

            if (!string.IsNullOrEmpty(plat))
            {
                return JsonSerializer.Deserialize<Platform>(plat) != null;
            }
            return false;
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;

            Random rnd = new Random();
            command.Id = rnd.Next(1,100000);

            var db = _redis.GetDatabase();
            var serialPlat = JsonSerializer.Serialize(command);

            db.HashSet($"hashcommand", new HashEntry[] 
                {new HashEntry("Command:" + command.Id, serialPlat)});
        }
        public Command? GetCommand(int platformId, int commandId)
        {
            var db = _redis.GetDatabase();

            var command = db.HashGet("hashcommand", "Command:" + commandId);

            if (!string.IsNullOrEmpty(command))
            {
                var desCommand = JsonSerializer.Deserialize<Command>(command);
                if (desCommand != null && desCommand.PlatformId == platformId)
                    return desCommand;
            }
            return null;
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            var commands = GetCommands();
            return commands.Where(e => e.PlatformId == platformId);
        }

        public IEnumerable<Command> GetCommands()
        {
            var db = _redis.GetDatabase();

            var completeSet = db.HashGetAll("hashcommand");
            var result = new List<Command>();
            if (completeSet.Length != 0)
            {
                foreach (var item in completeSet)
                {
                    var desCommand = JsonSerializer.Deserialize<Command>(item.Value);
                    if (desCommand != null)
                        result.Add(desCommand);
                }
            }

            return result;
        }

        public bool SaveChanges()
        {
            return true;
        }
    }
}