using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UKAPI.UMM;
using static UKAPI.UKAPI;

namespace UKAPI.Internal
{
    internal static class SaveFileHandler
    {
        private static Dictionary<string, Dictionary<string, string>> savedData = new Dictionary<string, Dictionary<string, string>>();
        private static FileInfo SaveFile = null;

        internal static void LoadData()
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            string path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar + "persistent mod data.json";
            Plugin.logger.LogInfo("Trying to load persistent mod data.json from " + path);
            SaveFile = new FileInfo(path);
            if (SaveFile.Exists)
            {
                using (StreamReader jFile = SaveFile.OpenText())
                {
                    savedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jFile.ReadToEnd());
                    if (savedData == null)
                        savedData = new Dictionary<string, Dictionary<string, string>>();
                    jFile.Close();
                }
            }
            else
            {
                Plugin.logger.LogInfo("Couldn't find a save file, making one now");
                SaveFile.Create();
            }
            KeyBindHandler.LoadKeyBinds();
            if (EnsureModData("UMM", "ModProfiles"))
                UltraModManager.LoadModProfiles();
            else
            {
                SetModData("UMM", "ModProfiles", "");
                SetModData("UMM", "CurrentModProfile", "Default");
            }
        }

        internal static void DumpFile()
        {
            if (SaveFile == null)
                return;
            Plugin.logger.LogInfo("Dumping keybinds");
            KeyBindHandler.DumpKeyBinds();
            Plugin.logger.LogInfo("Dumping mod profiles");
            UltraModManager.DumpModProfiles();
            Plugin.logger.LogInfo("Dumping mod persistent data file to " + SaveFile.FullName);
            File.WriteAllText(SaveFile.FullName, JsonConvert.SerializeObject(savedData));
        }

        /// <summary>
        /// Gets presistent mod data from a key and modname
        /// </summary>
        /// <param name="modName">The name of the mod to retrieve data from</param>
        /// <param name="key">The value you want</param>
        /// <returns>The mod data if found, otherwise null</returns>
        public static string RetrieveModData(string key, string modName)
        {
            if (savedData.ContainsKey(modName))
            {
                if (savedData[modName].ContainsKey(key))
                    return savedData[modName][key];
            }
            return null;
        }

        /// <summary>
        /// Adds persistent mod data from a key and mod name
        /// </summary>
        /// <param name="modName">The name of the mod to add data to</param>
        /// <param name="key">The key for the data</param>
        /// <param name="value">The data you want as a string, note you can only add strings</param>
        public static void SetModData(string modName, string key, string value)
        {
            if (!savedData.ContainsKey(modName))
            {
                Dictionary<string, string> newDict = new Dictionary<string, string>();
                newDict.Add(key, value);
                savedData.Add(modName, newDict);
            }
            else if (!savedData[modName].ContainsKey(key))
                savedData[modName].Add(key, value);
            else
                savedData[modName][key] = value;
        }

        /// <summary>
        /// Removes persistent mod data from a key and a mod name
        /// </summary>
        /// <param name="modName">The name of the mod to remove data from</param>
        /// <param name="key">The key for the data</param>
        public static void RemoveModData(string modName, string key)
        {
            if (savedData.ContainsKey(modName))
            {
                if (savedData[modName].ContainsKey(key))
                    savedData[modName].Remove(key);
            }
        }

        /// <summary>
        /// Checks if persistent mod data exists from a key and a mod name
        /// </summary>
        /// <param name="modName">The name of the mod to remove data from</param>
        /// <param name="key">The key for the data</param>
        public static bool EnsureModData(string modName, string key)
        {
            return savedData.ContainsKey(modName) && savedData[modName].ContainsKey(key);
        }
    }
}
