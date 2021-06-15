using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Discord;
using Discord.Gateway;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Console = Colorful.Console;

namespace FlushedSelfbot.Config
{
    internal class MessageLogger
    {
        private readonly string _config = Directory.GetCurrentDirectory() + @"\config\messagelogger.json";
        private readonly List<DiscordMessage> _messages = new List<DiscordMessage>();
        private readonly string _logFolder = Directory.GetCurrentDirectory() + @"\logs";
        private string _logFile;
        private bool _logToConsole;
        private bool _logToFile;
        private bool _logToNotification;

        private bool _logDeletedMessages = true;
        private bool _logGhostPings = true;
        private bool _logSentMessages;
        private bool _logEditedMessages;

        public void OnMessageReceived(DiscordMessage message)
        {
            _messages.Add(message);
            
            if (!_logSentMessages) return;
            Log(null, message);
        }

        public void OnMessageEdited(DiscordMessage message)
        {
            if (message.Content.Equals(" "))
                return;
            if (_messages.Any(discordMessage => discordMessage.Id == message.Id))
            {
                _messages.Remove(_messages.First(discordMessage => discordMessage.Id == message.Id));
                _messages.Add(message);
            }

            if (!_logEditedMessages) return;
            Log("Edited", message);
        }
        
        public void OnMessageDeleted(DiscordSocketClient client, DeletedMessage deleted)
        {
            var message = _messages.FirstOrDefault(discordMessage => discordMessage.Id == deleted.Id);
            if (message == null) return;

            if (message.Mentions.Any(user => user.Id == client.User.Id) && _logGhostPings)
            {
                Log("GhostPing", message);
                return;
            }
            
            if (!_logDeletedMessages) return;
            Log("Deleted", message);
        }

        private void Log(string type, DiscordMessage message)
        {
            var log =
                $"[{DateTime.Now:g}] {(type == null ? "" : $"[{type}] ")}[{Bot.Client.GetGuild(message.Guild).Name} " +
                $"#{Bot.Client.GetChannel(message.Channel.Id).Name}] {message.Author}: " +
                $"{message.Content}{(message.Attachment != null ? " (+ 1 attachment)" : "")}";

            if (_logToConsole) Console.WriteLine($"[Message Logger] {log}", Color.Aqua);
            if (_logToNotification) new ToastContentBuilder().AddText("Message Logger").AddText(log).Show();
            if (!_logToFile) return;

            Directory.CreateDirectory(_logFolder);

            if (_logFile == null)
            {
                var fileName = DateTime.Now.ToString("yyyy-MM-dd");
                var count = 0;

                while (File.Exists($@"{_logFolder}\{fileName}{(count == 0 ? "" : $"-{count}")}.txt"))
                    count++;

                _logFile = $@"{_logFolder}\{fileName}{(count == 0 ? "" : $"-{count}")}.txt";
            }

            using (var writer = File.AppendText(_logFile))
                writer.WriteLine(log);
        }

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

                    writer.WritePropertyName("logToConsole");
                    writer.WriteValue(_logToConsole);
                    
                    writer.WritePropertyName("logToFile");
                    writer.WriteValue(_logToFile);
                        
                    writer.WritePropertyName("logToNotification");
                    writer.WriteValue(_logToNotification);
                    
                    writer.WritePropertyName("settings");
                    writer.WriteStartObject();
                    {
                        writer.WritePropertyName("logSentMessages");
                        writer.WriteValue(_logSentMessages);

                        writer.WritePropertyName("logDeletedMessages");
                        writer.WriteValue(_logDeletedMessages);

                        writer.WritePropertyName("logEditedMessages");
                        writer.WriteValue(_logEditedMessages);

                        writer.WritePropertyName("logGhostPings");
                        writer.WriteValue(_logGhostPings);
                    }
                    writer.WriteEndObject();
                    
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

                        var logToConsole = (JValue) json.SelectToken("logToConsole");
                        if (logToConsole?.Value is bool logToConsoleValue)
                            _logToConsole = logToConsoleValue;

                        var logToFile = (JValue) json.SelectToken("logToFile");
                        if (logToFile?.Value is bool logToFileValue)
                            _logToFile = logToFileValue;
                        
                        var logToNotification = (JValue) json.SelectToken("logToNotification");
                        if (logToNotification?.Value is bool logToNotificationValue)
                            _logToNotification = logToNotificationValue;
                        
                        var settings = json.SelectToken("settings");
                        if (settings == null) return;
                        
                        var logSentMessages = (JValue) settings.SelectToken("logSentMessages");
                        if (logSentMessages?.Value is bool logSentMessagesValue)
                            _logSentMessages = logSentMessagesValue;

                        var logDeletedMessages = (JValue) settings.SelectToken("logDeletedMessages");
                        if (logDeletedMessages?.Value is bool logDeletedMessagesValue)
                            _logDeletedMessages = logDeletedMessagesValue;

                        var logEditedMessages = (JValue) settings.SelectToken("logEditedMessages");
                        if (logEditedMessages?.Value is bool logEditedMessagesValue)
                            _logEditedMessages = logEditedMessagesValue;

                        var logGhostPings = (JValue) settings.SelectToken("logGhostPings");
                        if (logGhostPings?.Value is bool logGhostPingsValue)
                            _logGhostPings = logGhostPingsValue;
                    }
                }
                Save();
            }
            catch (IOException e)
            {
                Console.WriteLine(e, Color.Red);
                File.Delete(_config);
            }
        }
    }
}
