using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Colorful;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlushedSelfbot.Config
{
    internal class AutoFeur
    {
        private static readonly string ConfigFolder = Directory.GetCurrentDirectory() + @"\config";
        private readonly string _config = ConfigFolder + @"\autofeur.json";
        public readonly Dictionary<string, string[]> Map = new Dictionary<string, string[]>();
        public bool Enabled;

        private void Save()
        {
            Directory.CreateDirectory(ConfigFolder);
            
            try
            {
                var stringWriter = new StringWriter();
                
                using (JsonWriter writer = new JsonTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;
                    
                    writer.WriteStartObject();

                    writer.WritePropertyName("enabled");
                    writer.WriteValue(Enabled);
                    
                    writer.WritePropertyName("feurs");
                    writer.WriteStartArray();

                    foreach (var feur in Map)
                    {
                        writer.WriteStartObject();
                        
                        writer.WritePropertyName("response");
                        writer.WriteValue(feur.Key);
                        
                        writer.WritePropertyName("togglers");
                        writer.WriteStartArray();

                        foreach (var toggler in feur.Value)
                        {
                            writer.WriteValue(toggler);
                        }

                        writer.WriteEndArray();
                        
                        writer.WriteEndObject();
                    }
                    
                    writer.WriteEndArray();

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
                Map.Add("feur", new []{"quoi", "koi", "qoi", "koa", "kwa", "pk", "pq"});
                Map.Add("stiti", new[] {"oui", "ui"});
                Map.Add("bril", new[] {"nn", "non"});
                Map.Add("vabo", new[] {"la"});
                Map.Add("stern", new[] {"oue", "oe", "oé", "oué", "ouais", "ouai"});
                Map.Add("on", new[] {"mais"});
                
                Save();
                return;
            }

            Map.Clear();
            
            try
            {
                using (var streamReader = File.OpenText(_config))
                {
                    using (var reader = new JsonTextReader(streamReader))
                    {
                        var json = JObject.Load(reader);

                        var enabled = (JValue) json.SelectToken("enabled");
                        if (enabled?.Value is bool enabledValue)
                            Enabled = enabledValue;
                        
                        var feurs = (JArray) json.SelectToken("feurs");
                        if (feurs == null) return;
                        foreach (var obj in feurs)
                        {
                            if (!(obj is JObject)) continue;
                            var response = (JValue) obj.SelectToken("response");
                            var togglers = (JArray) obj.SelectToken("togglers");

                            if (response == null || togglers == null || response.Value == null) continue;
                            Map.Add((string) response.Value, togglers.Values<string>().ToArray());
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e, Color.Red);
                File.Delete(_config);
            }
        }
    }
}
