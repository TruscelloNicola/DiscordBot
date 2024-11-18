using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using APIIntegratedBot.Api;
using Microsoft.AspNetCore.Hosting;


namespace APIIntegratedBot
{

    class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        public string[] badWordList = { "badword1", "badword2" }; //PlaceholderBadwordList

        static void Main(string[] args)
            => new Program().RunBotAsync().GetAwaiter().GetResult();

        public async Task RunBotAsync()
        {
            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All
            };
            _client = new DiscordSocketClient(config);
            _commands = new CommandService();

            _client.Log += Log;

            if (File.Exists("./BadWordJSON.json"))
            {
                var content = File.ReadAllText("./BadWordJSON.json");

                string[] temp = System.Text.Json.JsonSerializer.Deserialize<string[]>(content);

                badWordList = temp;
            }

            _client.MessageReceived += BadWordDetector;

            await _client.LoginAsync(TokenType.Bot, "X");
            await _client.StartAsync();

            _client.Ready += OnReady;

            await RegisterCommandsAsync();
            await CreateHostBuilder(_client).Build().RunAsync();



            //_client.Ready += OnReady;
            await Task.Delay(-1);
        }

        public async Task RegisterCommandsAsync()
        {
            _client.Log += Log;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
        private async Task OnReady()
        {
            Console.WriteLine("Bot is connected!");
            var channel = _client.GetChannel(1186215061170176073) as IMessageChannel;
            await channel.SendMessageAsync("Bot is connected!");
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

		private const ulong MutedRoleId = 1196362522849181827;
		private async Task BadWordDetector(SocketMessage message)
		{
			string[] badWordList = { "badword1", "badword2" };

			if (message.Author.IsBot)
			{
				Console.WriteLine("Bot Detected, message ignored");
				return;
			}
			Console.WriteLine(message.Author.Username);
			Console.WriteLine($"Received message content: {message.Content}");

			foreach (string badWord in badWordList)
			{
				if (message.Content.ToLower().Contains(badWord.ToLower()))
				{
					Console.WriteLine($"Bad word found from {message.Author.Username}\nBad word: {message.Content}");

					try
					{
						await message.DeleteAsync();

						var channel = _client.GetChannel(1186215061170176073) as IMessageChannel;
						if (channel != null)
						{
							var botResponse = await channel.SendMessageAsync($"Bad word detected in a message by {message.Author.Username}. The message was deleted, and {message.Author.Username} has been muted for 30 seconds.");

							var mutedRole = (message.Channel as SocketGuildChannel)?.Guild.GetRole(MutedRoleId);
							var user = message.Author as SocketGuildUser;

							await user.AddRoleAsync(mutedRole);
							await BotResponseCleanup(botResponse);

							await Task.Delay(30000);

							await user.RemoveRoleAsync(mutedRole);

							var botUnmuteMessage = await channel.SendMessageAsync($"{message.Author.Username} has been unmuted");

							await Task.Delay(5000);

							await botUnmuteMessage.DeleteAsync();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Error occurred: {ex.Message}");
					}

					return;
				}
			}
		}

		private async Task BotResponseCleanup(IUserMessage botResponse)
		{
			try
			{
				var channel = botResponse.Channel as IMessageChannel;
				var message = await channel.GetMessageAsync(botResponse.Id);

				if (message != null)
				{
					await Task.Delay(5000);

					await message.DeleteAsync();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error deleting bot response: {ex.Message}");
			}
		}

		public static IHostBuilder CreateHostBuilder(DiscordSocketClient discordSocketClient, string[] args = null) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().ConfigureServices(services =>
                    {
                        services.AddSingleton(discordSocketClient);
                    });
                });

    }

}