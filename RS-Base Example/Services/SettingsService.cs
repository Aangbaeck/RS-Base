using System;
using System.IO;
using Newtonsoft.Json;
using RS_Base.Models;
using RS_Base.Net.Helper;
using Serilog;

namespace RS_Base.Services
{
    public class SettingsService
    {
        public Settings Settings { get; set; } = new Settings();

        public SettingsService()
        {
            LoadSettings();
        }



        private void LoadSettings()
        {
            try
            {
                if (File.Exists(Common.SettingsPath))
                {
                    var json = File.ReadAllText(Common.SettingsPath);
                    Settings = JsonConvert.DeserializeObject<Settings>(json);
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
