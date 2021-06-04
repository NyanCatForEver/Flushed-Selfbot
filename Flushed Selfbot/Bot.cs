using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Gateway;
using FlushedSelfbot.Command;
using FlushedSelfbot.Config;
using Microsoft.Toolkit.Uwp.Notifications;
using Console = Colorful.Console;

namespace FlushedSelfbot
{
    internal static class Bot
    {
        public static DiscordSocketClient Client;
        public static CommandManager CommandManager;
        public static readonly ConfigManager ConfigManager = new ConfigManager();
        private static readonly AutoFeur AutoFeur = new AutoFeur();
        private static bool _loggedIn;
        public static readonly Dictionary<ulong, List<ulong>> CachedMembers = new Dictionary<ulong, List<ulong>>();

        public const string Name = "Flushed Selfbot",
            Version = "0.1-BETA",
            FlushedUrl = "https://cdn.discordapp.com/attachments/844313460330332171/844986513791909939/flushed.png";

        public static readonly PrivateFontCollection FontCollection = new PrivateFontCollection();

        private const string AsciiName =
            "    ███████╗██╗     ██╗   ██╗ ██████╗██╗  ██╗███████╗██████╗          ██████╗███████╗██╗     ███████╗██████╗  █████╗ ████████╗\n" +
            "    ██╔════╝██║     ██║   ██║██╔════╝██║  ██║██╔════╝██╔══██╗        ██╔════╝██╔════╝██║     ██╔════╝██╔══██╗██╔══██╗╚══██╔══╝\n" +
            "    █████╗  ██║     ██║   ██║╚█████╗ ███████║█████╗  ██║  ██║        ╚█████╗ █████╗  ██║     █████╗  ██████╦╝██║  ██║   ██║   \n" +
            "    ██╔══╝  ██║     ██║   ██║ ╚═══██╗██╔══██║██╔══╝  ██║  ██║         ╚═══██╗██╔══╝  ██║     ██╔══╝  ██╔══██╗██║  ██║   ██║   \n" +
            "    ██║     ███████╗╚██████╔╝██████╔╝██║  ██║███████╗██████╔╝        ██████╔╝███████╗███████╗██║     ██████╦╝╚█████╔╝   ██║   \n" +
            "    ╚═╝     ╚══════╝ ╚═════╝ ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═════╝         ╚═════╝ ╚══════╝╚══════╝╚═╝     ╚═════╝  ╚════╝    ╚═╝   \n";

        private static void Main()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => ConfigManager.Save();

            new Thread(() =>
            {
                while (true)
                {
                    if (Console.WindowWidth < AsciiName.Split('\n')[0].Length + 4)
                        Console.WindowWidth = AsciiName.Split('\n')[0].Length + 4;
                }
            }).Start();


            Console.Title = Name + " " + Version + " | Initializing";
            ClearConsole();

            AutoFeur.Load();
            ConfigManager.Load();
            Console.CursorVisible = false;

            var bytes = (byte[]) Resources.ResourceManager.GetObject("Comfortaa");

            if (bytes != null)
            {
                var intPtr = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, intPtr, bytes.Length);
                FontCollection.AddMemoryFont(intPtr, bytes.Length);
                Marshal.FreeHGlobal(intPtr);
            }

            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                ApiVersion = 7,
                Intents = null,
                RetryOnRateLimit = true
            });
            CommandManager = new CommandManager();
            Client.OnLoggedIn += OnLoggedIn;
            Client.OnMessageReceived += OnMessageReceived;
            Client.OnMessageEdited += OnMessageEdited;

            Client.OnUserJoinedGuild += (client, args) =>
            {
                if (!CachedMembers.ContainsKey(args.Member.Guild.Id))
                    CachedMembers.Add(args.Member.Guild.Id, new List<ulong>());
                CachedMembers[args.Member.Guild.Id].Add(args.Member.User.Id);
            };
            Client.OnUserLeftGuild += (client, args) =>
            {
                if (CachedMembers.ContainsKey(args.Member.Guild.Id))
                    CachedMembers[args.Member.Guild.Id].Remove(args.Member.User.Id);
            };

            try
            {
                Client.Login(ConfigManager.Token);

                Console.Title = Name + " " + Version + " | Logging in";
                Console.Write("Logging in", Color.Aqua);
                
                for (var i = 1; !_loggedIn; i++)
                {
                    Console.Title += ".";
                    Console.Write(".", Color.Aqua);
                    Thread.Sleep(200);

                    if (i < 3)
                        continue;

                    i = 0;
                    Console.Title = Name + " " + Version + " | Logging in";
                    Console.Write("\b \b\b \b\b \b");
                }

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine(
                    $"Logged in as {Client.User}. Prefix: {ConfigManager.Prefix}. {CommandManager.Commands.Count} commands loaded.",
                    Color.LawnGreen);
                Console.Title = $"{Name} {Version} | {Client.User}";
                
                new ToastContentBuilder()
                    .AddText($"Logged in as {Client.User}.")
                    .Show();
            }
            catch (InvalidTokenException)
            {
                Console.WriteLine("Failed to login into account. Please check the token.", Color.Red);
                Console.Title = Name + " " + Version + " | Failed to login";
                Console.Beep(1000, 400);
            }
            catch (DiscordConnectionException)
            {
                Console.WriteLine("Failed to connect to Discord. Please check your internet connection and retry.",
                    Color.Red);
                Console.Title = Name + " " + Version + " | Failed to connect to Discord";
                Console.Beep(1000, 400);
            }

            Thread.Sleep(-1);
        }

        private static void OnMessageEdited(DiscordSocketClient client, MessageEventArgs args)
        {
            if (args.Message.Author.User.Id != Client.User.Id)
                return;

            if (ConfigManager.DeleteEmbeds && args.Message.Embed != null && args.Message.Embed.Footer.Text.Equals(Name))
            {
                new Task(() =>
                {
                    Thread.Sleep(ConfigManager.DeleteEmbedsDelay * 1000);
                    args.Message.Delete();
                }).Start();
            }
        }

        private static void OnLoggedIn(DiscordSocketClient client, LoginEventArgs args)
        {
            if (_loggedIn)
            {
                Console.WriteLine("Reconnected to account", Color.LawnGreen);
                return;
            }

            _loggedIn = true;
        }

        private static void OnMessageReceived(DiscordSocketClient client, MessageEventArgs args)
        {
            var content = args.Message.Content;
            if (args.Message.Author.User.Id != Client.User.Id)
            {
                if (!AutoFeur.Enabled) return;
                var alphabetic = content.Where(ch => char.IsLetter(ch) || char.IsDigit(ch))
                    .Aggregate("", (current, ch) => current + ch);

                foreach (var feur in AutoFeur.Map.Where(feur =>
                    feur.Value.Any(toggler => alphabetic.ToLower().EndsWith(toggler.ToLower()))))
                {
                    args.Message.Channel.SendMessage(feur.Key);
                    Console.WriteLine(
                        $"[{DateTime.Now:g}] {feur.Key.First().ToString().ToUpper() + feur.Key.Substring(1)}ed " +
                        args.Message.Author + " in guild \"" + Client.GetGuild(args.Message.Guild).Name +
                        "\" in channel \"" +
                        Client.GetChannel(args.Message.Channel.Id).Name + "\".", Color.Aqua);
                }

                return;
            }

            if (ConfigManager.DeleteEmbeds && args.Message.Embed != null && args.Message.Embed.Footer.Text.Equals(Name))
            {
                new Task(() =>
                {
                    Thread.Sleep(ConfigManager.DeleteEmbedsDelay * 1000);
                    args.Message.Delete();
                }).Start();
            }

            if (!content.StartsWith(ConfigManager.Prefix))
                return;

            var command = CommandManager.OnMessageReceived(client, args.Message);
            if (command != null)
                Console.WriteLine($"[{DateTime.Now:g}] Used command {command.Name}", Color.Aqua);
        }

        private static void ClearConsole()
        {
            Console.Clear();
            Console.WriteWithGradient("\n" + AsciiName + "\n", Color.Crimson, Color.DarkOrange, 8);
        }
    }
}