using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RS_Base.Models;
using RS_Base.Net.Helper;
using Serilog;

namespace RS_Base.Net.Model
{
    public class SettingsService
    {
        public Settings Settings { get; set; } = new Settings();

       
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        private static SettingsService? instance = null;
        private static readonly object padlock = new object();

        SettingsService()
        {
            LoadSettings();
        }

        public static SettingsService Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SettingsService();
                    }
                    return instance;
                }
            }
        }



        private void LoadSettings()
        {
            try
            {
                if (File.Exists(Common.SettingsPath))
                {
                    var json = File.ReadAllText(Common.SettingsPath);
                    var serializerSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
                    Settings = JsonConvert.DeserializeObject<Settings>(json, serializerSettings);
                }
                else
                {
                    SaveSettings();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Settings file is corrupt");
            }

        }



        public void SaveSettings()
        {
            try
            {
                var dir = Path.GetDirectoryName(Common.SettingsPath) ?? "";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var results = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                File.WriteAllText(Common.SettingsPath, results);
            }



            catch (Exception e)
            {
                Log.Error(e, "Could not save settings.");
            }
        }
    }





}



