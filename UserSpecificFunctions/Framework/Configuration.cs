using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UserSpecificFunctions.Framework
{
    /// <summary>Represents the configuration file.</summary>
    public class Configuration
    {
        /// <summary>The maximum prefix length.</summary>
        public int PrefixLength { get; set; } = 20;

        /// <summary>The maximum suffix length.</summary>
        public int SuffixLength { get; set; } = 20;

        /// <summary>An array of words players are prohibited to use in their chat tags.</summary>
        public string[] UnAllowedWords = new string[] { "Ass", "Asshole", "Fuck", "Fucktard", "Shit", "Shithead", "Fucker", "Motherfucker" };

        /// <summary>
        /// Returns a <see cref="Configuration"/> instance from the given path.
        /// In case the path does not exist a new <see cref="Configuration"/> instance is created and written.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>A <see cref="Configuration"/> instance.</returns>
        public static Configuration ReadOrWrite(string path)
        {
            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path));
            }

            var _config = new Configuration();
            File.WriteAllText(path, JsonConvert.SerializeObject(_config, Formatting.Indented));
            return _config;
        }
    }
}
