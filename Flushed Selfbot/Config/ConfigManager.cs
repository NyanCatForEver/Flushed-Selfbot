using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Discord;
using FlushedSelfbot.Command.Commands.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Console = Colorful.Console;

namespace FlushedSelfbot.Config
{
    internal class ConfigManager
    {
        private readonly string _config = Directory.GetCurrentDirectory() + @"\config\config.json";
        public string Prefix = "=";
        public string Token;
        public bool DeleteEmbeds = true;
        public int DeleteEmbedsDelay = 10;
        public bool DisableEmbeds;

        public void Save()
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\config");
            
            try
            {
                var stringWriter = new StringWriter();
                using (JsonWriter writer = new JsonTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;
                    
                    writer.WriteStartObject();
                    {
                        writer.WritePropertyName("prefix");
                        writer.WriteValue(Prefix);
                        writer.WritePropertyName("token");
                        writer.WriteValue(Token);

                        var webhook = Webhook.DiscordWebhook;
                        writer.WritePropertyName("webhook");
                        writer.WriteValue(webhook == null ? null : $"https://discord.com/api/webhooks/{webhook.Id}/{webhook.Token}");

                        writer.WritePropertyName("safemode");
                        writer.WriteStartObject();
                        {
                            writer.WritePropertyName("deleteEmbeds");
                            writer.WriteStartObject();
                            {
                                writer.WritePropertyName("enabled");
                                writer.WriteValue(DeleteEmbeds);
                                writer.WritePropertyName("afterSeconds");
                                writer.WriteValue(DeleteEmbedsDelay);
                            }
                            writer.WriteEndObject();
                            
                            writer.WritePropertyName("disableEmbeds");
                            writer.WriteValue(DisableEmbeds);
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                }
                stringWriter.Close();
                
                File.WriteAllText(_config, stringWriter.ToString());
            }
            catch (IOException e)
            {
                Console.WriteLine(e, Color.Red);
            }
        }

        public void Load()
        {
            if (!File.Exists(_config))
            {
                Console.Beep(1000, 400);
                Console.Write("Paste your token here: ", Color.Aqua);
                var token = "";
                ConsoleKey key;
                do
                {
                    var keyInfo = Console.ReadKey(true);
                    key = keyInfo.Key;
                    if (key == ConsoleKey.Backspace && token.Length > 0)
                    {
                        Console.Write("\b \b");
                        token = token.Remove(token.Length - 1);
                    }
                    else if (!char.IsControl(keyInfo.KeyChar))
                    {
                        Console.Write("*", Color.DarkSlateGray);
                        token += keyInfo.KeyChar;
                    }
                } while (key != ConsoleKey.Enter);

                Token = token;
                Console.WriteLine("\nSet token to " + token.Substring(0, 3) + new Regex("\\S")
                    .Replace(token.Substring(3), "*"), Color.SpringGreen);

                Save();
                return;
            }

            try
            {
                using (var streamReader = File.OpenText(_config))
                {
                    using (var reader = new JsonTextReader(streamReader))
                    {
                        var json = JObject.Load(reader);
                        {
                            var prefix = (JValue) json.SelectToken("prefix");
                            if (prefix?.Value is string prefixValue)
                                Prefix = prefixValue;

                            var token = (JValue) json.SelectToken("token");
                            if (token?.Value is string tokenValue)
                                Token = tokenValue;

                            var webhook = (JValue) json.SelectToken("webhook");
                            if (webhook?.Value is string url)
                            {
                                var cut = url.Substring(url.IndexOf("api/webhooks/", StringComparison.OrdinalIgnoreCase) +
                                                            "api/webhooks/".Length);
                                var id = ulong.Parse(cut.Split('/')[0]);
                                var webhookToken = cut.Split('/')[1];
                                try
                                {
                                    Webhook.DiscordWebhook = new DiscordDefaultWebhook(id, webhookToken);
                                }
                                catch (DiscordHttpException) {}
                            }

                            var safemode = (JObject) json.SelectToken("safemode");
                            {
                                var deleteEmbeds = safemode?.SelectToken("deleteEmbeds");
                                {
                                    var enabled = ((JValue) deleteEmbeds?.SelectToken("enabled"))?.Value;
                                    if (enabled != null) DeleteEmbeds = (bool) enabled;
                                    var delay = ((JValue) deleteEmbeds?.SelectToken("afterSeconds"))?.Value;
                                    if (delay != null) DeleteEmbedsDelay = (int) ((long) delay);
                                }

                                var disableEmbeds = ((JValue) safemode?.SelectToken("disableEmbeds"))?.Value;
                                if (disableEmbeds != null)
                                    DisableEmbeds = (bool) disableEmbeds;
                            }
                        }
                    }
                }
                Save();
            }
            catch (IOException e)
            {
                Console.WriteLine(e, Color.Red);
                File.Delete(_config);
            }
            
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => Save();
        }
    }
}
