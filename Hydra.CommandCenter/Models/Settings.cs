using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Hydra.CommandCenter
{
    public class Settings
    {
        const string Path = "settings.json";

        public static Settings Instance { get; private set; }

        public List<string> Hosts { get; set; }
        public List<string> Packages { get; set; }
        public List<string> Projects { get; set; }

        public string LastParameterInput { get; set; }

        static Settings()
        {
            if( File.Exists( Path ) )
            {
                try
                {
                    var contents = File.ReadAllText( Path );
                    Instance = JsonConvert.DeserializeObject<Settings>( contents );
                }
                catch { }
            }

            if( Instance == null )
            {
                Instance = new Settings();
            }
        }

        private Settings() 
        {
            Hosts = new List<string>();
            Packages = new List<string>();
            Projects = new List<string>();
        }

        public static void Save()
        {
            var data = JsonConvert.SerializeObject( Instance );
            File.WriteAllText( Path, data );
        }
    }
}
