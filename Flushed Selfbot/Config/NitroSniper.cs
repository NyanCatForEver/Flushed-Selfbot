using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Discord;
using Discord.Gateway;
using Leaf.xNet;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Console = Colorful.Console;

namespace FlushedSelfbot.Config
{
    internal class NitroSniper
    {
        private readonly string _config = Directory.GetCurrentDirectory() + @"\config\nitrosniper.json";
        private bool _enabled;
        private bool _notifyWhenSniped = true;
        private bool _thankWhenSniped;
        private string _thanksMessage = "Thanks for the Nitro {mention}!";

        private void Save()
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
                        writer.WritePropertyName("enabled");
                        writer.WriteValue(_enabled);

                        writer.WritePropertyName("notifyWhenSniped");
                        writer.WriteValue(_notifyWhenSniped);

                        writer.WritePropertyName("thankWhenSniped");
                        writer.WriteStartObject();
                        {
                            writer.WritePropertyName("enabled");
                            writer.WriteValue(_thankWhenSniped);
                            
                            writer.WritePropertyName("message");
                            writer.WriteValue(_thanksMessage);
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
                            var enabled = ((JValue) json.SelectToken("enabled"))?.Value;
                            if (enabled != null) _enabled = (bool) enabled;
                            var notifyOnSnipe = ((JValue) json.SelectToken("notifyWhenSniped"))?.Value;
                            if (notifyOnSnipe != null) _notifyWhenSniped = (bool) notifyOnSnipe;

                            var thankWhenSniped = json.SelectToken("thankWhenSniped");
                            {
                                var thankEnabled = ((JValue) thankWhenSniped?.SelectToken("enabled"))?.Value;
                                if (thankEnabled != null) _thankWhenSniped = (bool) thankEnabled;
                                var message = ((JValue) thankWhenSniped?.SelectToken("message"))?.Value;
                                if (message != null) _thanksMessage = (string) message;
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

        public void OnMessageReceived(DiscordSocketClient client, DiscordMessage message)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var match = Regex.Match(message.Content, @"\b(discord(app)?\.com\/gifts)\/[0-9a-z]+|(discord\.gift)\/[0-9a-z]+\b", RegexOptions.IgnoreCase);
            if (!_enabled || !match.Success) return;
            
            var code = match.Value.Split('/')[match.Value.Split('/').Length - 1];
            var status = -1;
            try
            {
                dynamic token = client.HttpClient
                    .GetAsync("/entitlements/gift-codes/" + code +
                              "?with_application=false&with_subscription_plan=true").GetAwaiter().GetResult()
                    .Object;
                if (token.uses >= token.max_uses)
                    status = 0;
                else if (((string) token.store_listing.sku.name).ContainsInsensitive("nitro"))
                {
                    client.RedeemGift(code);
                    status = 1;
                }
            }
            catch (DiscordHttpException)
            {
            }

            var time = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(
                $"[{DateTime.Now:g}] [Nitro sniper] " +
                (status == 1 ? "Redeemed" : status == 0 ? "Found already redeemed" : "Found invalid") +
                $" gift code \"{code}\" from {message.Author} in {client.GetGuild(message.Guild).Name} " +
                $"#{client.GetChannel(message.Channel.Id).Name} in {time} milliseconds",
                status == 1 ? Color.LimeGreen : Color.Red);

            if (status != 1) return;
            
            if (_thankWhenSniped)
                message.Channel.SendMessage(_thanksMessage
                    .Replace("{name}", message.Author.User.Username)
                    .Replace("{tag}", message.Author.ToString())
                    .Replace("{mention}", message.Author.User.AsMessagable()));

            if (_notifyWhenSniped)
                new ToastContentBuilder()
                    .AddText("Nitro sniper")
                    .AddText(
                        $"Redeemed gift code \"{code}\" from {message.Author} in {client.GetGuild(message.Guild).Name} " +
                        $"#{client.GetChannel(message.Channel.Id).Name} in {time} milliseconds")
                    .Show();
        }
    }
}
