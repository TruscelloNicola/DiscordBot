using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
// Api/ApiController.cs
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace APIIntegratedBot.Api
{

    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private DiscordSocketClient _client;

        public ApiController(DiscordSocketClient client)
        {
            _client = client;
        }

        [HttpGet("guildMember")]
        public IActionResult GetGuildMembers()
        {

            var guild = _client.GetGuild(1186215061170176070);

            // Check if the guild is null
            if (guild == null)
            {
                Console.WriteLine($"Guild not found for ID: {1186215061170176070}");
                return NotFound("Guild not found");
            }

            guild.DownloadUsersAsync(); // Ensure all users are downloaded


            // Check if the Users collection is null
            if (guild.Users == null)
            {
                Console.WriteLine($"Users collection is null for guild ID: {1186215061170176070}");
                return NotFound("Guild users not available");
            }

            // Retrieve usernames from guild users
            var users = guild.Users.Select(u => u.Id + "," + u.Username).ToList();

            return Ok(users);
        }


        [HttpGet("KickSomeone")]
        public IActionResult KickMember([FromQuery] string Id)
        {
            // Get guild context
            var guild = _client.GetGuild(1186215061170176070);
            // Get user
            var user = guild.Users.FirstOrDefault(x => $"{x.Id}" == Id);

            try
            {
                // Try kicking them
                guild.GetUser(user.Id).KickAsync();
                Console.WriteLine($"User {Id} kicked successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error kicking user {Id}: {ex.Message}");
            }

            return Ok();
        }

        [HttpGet("BanSomeone")]
        public IActionResult BanMember([FromQuery] string Id)
        {

            var guild = _client.GetGuild(1186215061170176070);

            var user = guild.Users.FirstOrDefault(x => $"{x.Id}" == Id);

            try
            {
                guild.GetUser(user.Id).BanAsync();
                Console.WriteLine($"User {Id} kicked successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error kicking user {Id}: {ex.Message}");
            }

            return Ok();
        }

        public class BadWordsRequest
        {
            public List<string> BadWords { get; set; }
        }

        [HttpPost("SetBadWords")]
        public IActionResult SetBadWords([FromBody] BadWordsRequest request)
        {
            if (request == null || request.BadWords == null)
            {
                return BadRequest("Bad words are null.");
            }

            if (System.IO.File.Exists("./BadWordJSON.json"))
            {
                System.IO.File.Delete("./BadWordJSON.json");
            }

            //For Program.cs to read upon startup
            string badWordsJson = JsonConvert.SerializeObject(request.BadWords);
            System.IO.File.WriteAllText("./BadWordJSON.json", badWordsJson);

            return Ok();
        }


    }

}
