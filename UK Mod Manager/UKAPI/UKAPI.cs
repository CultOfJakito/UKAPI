using System;
using System.Collections.Generic;
using UnityEngine;
using UKAPI.Internal;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Diagnostics;
using UKAPI.PowerUps;
using UKAPI.UMM;
using UnityEngine.AddressableAssets;

namespace UKAPI
{
    public static class UKAPI
    {
        public static bool IsDevBuild { get; internal set; }
        public static bool IsOutdated { get; internal set; }
        private static List<string> disableCybergrindReasons = new List<string>();

        /// <summary>
        /// Returns whether or not leaderboard submissions are allowed.
        /// </summary>
        public static bool CanSubmitCybergrindScore
        {
            get
            {
                return disableCybergrindReasons.Count == 0;
            }
        }

        /// <summary>
        /// Returns a clone of all found <see cref="ModInformation"/> instances.
        /// </summary>
        public static Dictionary<string, ModInformation> AllModInfoClone => UltraModManager.foundMods.ToDictionary(entry => entry.Key, entry => entry.Value);

        /// <summary>
        /// Returns a clone of all loaded <see cref="ModInformation"/> instances.
        /// </summary>
        public static Dictionary<string, ModInformation> AllLoadedModInfoClone => UltraModManager.allLoadedMods.ToDictionary(entry => entry.Key, entry => entry.Value);

        /// <summary>
        /// Initializes the API by loading the save file and common asset bundle
        /// </summary>
        internal static void Initialize()
        {
            Stopwatch watch = new Stopwatch();
            Plugin.logger.LogMessage("Beginning UKAPI");

            PowerUpPickupBuilder.DualWieldPrefab = Addressables
                .LoadAssetAsync<GameObject>("Assets/Prefabs/Levels/DualWieldPowerup.prefab")
                .WaitForCompletion();

            string[] launchArgs = Environment.GetCommandLineArgs();
            if (launchArgs != null)
            {
                foreach (string str in launchArgs)
                {
                    if (str.Contains("disable_mods"))
                    {
                        Plugin.logger.LogMessage("Not starting UMM due to launch arg disabling it.");
                        goto EndInitialization;
                    }
                }
            }
            SaveFileHandler.LoadData();
            UltraModManager.InitializeManager();
            if (launchArgs != null)
            {
                foreach (string str in launchArgs)
                {
                    if (str != null && (str.Contains("sandbox") || str.Contains("uk_construct")))
                    {
                        Plugin.logger.LogMessage("Launch argument detected: " + str + ", loading into the sandbox!");
                        SceneManager.LoadScene("uk_construct");
                    }
                    else
                    {
                        Plugin.logger.LogMessage("Launch argument detected: " + str + ", but is has no use with UKAPI!");
                    }
                }
            }
            
            EndInitialization:
            watch.Stop();
            Plugin.logger.LogMessage("UMM initialization completed in " + watch.ElapsedMilliseconds + "ms");
        }


        internal static void Update()
        {
            foreach (UKKeyBind bind in KeyBindHandler.moddedKeyBinds.Values.ToList())
                bind?.CheckEvents(); // Always null check :P
        }

        /// <summary>
        /// Disables CyberGrind score submission, CyberGrind submissions can only be re-enabled if nothing else disables it
        /// </summary>
        /// <param name="reason">Why CyberGrind is disabled, if you want to reenable it later you can do so by removing the reason</param>
        public static void DisableCyberGrindSubmission(string reason)
        {
            if (!disableCybergrindReasons.Contains(reason))
                disableCybergrindReasons.Add(reason);
        }

        /// <summary>
        /// Removes a Cybergrind disable reason if found, Cybergrind score submissions will only be enabled if there aren't any reasons to disable it
        /// </summary>
        /// <param name="reason">The reason to remove</param>
        public static void RemoveDisableCyberGrindReason(string reason)
        {
            if (disableCybergrindReasons.Contains(reason))
                disableCybergrindReasons.Remove(reason);
            else
                Plugin.logger.LogError("Tried to remove cg reason " + reason + " but could not find it!");
        }

        /// <summary>
        /// Enumerated version of the Ultrakill scene types
        /// </summary>
        public enum UKLevelType { Intro, MainMenu, Level, Endless, Sandbox, Credits, Custom, Intermission, Secret, PrimeSanctum, Unknown }

        /// <summary>
        /// Returns the current level type
        /// </summary>
        public static UKLevelType CurrentLevelType = UKLevelType.Intro;

        /// <summary>
        /// Returns the currently active ultrakill scene name.
        /// </summary>
        public static string CurrentSceneName { get; private set; } = "";


        /// <summary>
        /// Invoked whenever the current level type is changed.
        /// </summary>
        /// <param name="uKLevelType">The type of level that was loaded.</param>
        public delegate void OnLevelChangedHandler(UKLevelType uKLevelType);

        /// <summary>
        /// Invoked whenever the current level type is changed.
        /// </summary>
        public static OnLevelChangedHandler OnLevelTypeChanged;

        /// <summary>
        /// Invoked whenever the scene is changed.
        /// </summary>
        public static OnLevelChangedHandler OnLevelChanged;

        //Perhaps there is a better way to do this.
        private static void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            string sceneName = scene.name;

            if (scene != SceneManager.GetActiveScene())
                return;

            UKLevelType newScene = GetUKLevelType(sceneName);

            if (newScene != CurrentLevelType)
            {
                CurrentLevelType = newScene;
                CurrentSceneName = scene.name;
                OnLevelTypeChanged?.Invoke(newScene);
            }

            OnLevelChanged?.Invoke(CurrentLevelType);
        }

        //Perhaps there is a better way to do this. Also this will most definitely cause problems in the future if PITR or Hakita rename any scenes.

        /// <summary>
        /// Gets enumerated level type from the name of a scene.
        /// </summary>
        /// <param name="sceneName">Name of the scene</param>
        /// <returns></returns>
        public static UKLevelType GetUKLevelType(string sceneName)
        {
            sceneName = (sceneName.Contains("P-")) ? "Sanctum" : sceneName;
            sceneName = (sceneName.Contains("-S")) ? "Secret" : sceneName;
            sceneName = (sceneName.Contains("Level")) ? "Level" : sceneName;
            sceneName = (sceneName.Contains("Intermission")) ? "Intermission" : sceneName;

            switch (sceneName)
            {
                case "Main Menu":
                    return UKLevelType.MainMenu;
                case "Custom Content":
                    return UKLevelType.Custom;
                case "Intro":
                    return UKLevelType.Intro;
                case "Endless":
                    return UKLevelType.Endless;
                case "uk_construct":
                    return UKLevelType.Sandbox;
                case "Intermission":
                    return UKLevelType.Intermission;
                case "Level":
                    return UKLevelType.Level;
                case "Secret":
                    return UKLevelType.Secret;
                case "Sanctum":
                    return UKLevelType.PrimeSanctum;
                case "Credits":
                    return UKLevelType.Credits;
                default:
                    return UKLevelType.Unknown;
            }
        }

        /// <summary>
        /// Returns true if the current scene is playable.
        /// This will return false for all secret levels.
        /// </summary>
        /// <returns></returns>
        public static bool InLevel()
        {
            bool inNonPlayable = (CurrentLevelType == UKLevelType.MainMenu || CurrentLevelType == UKLevelType.Intro || CurrentLevelType == UKLevelType.Intermission || CurrentLevelType == UKLevelType.Secret || CurrentLevelType == UKLevelType.Unknown);
            return !inNonPlayable;
        }

        /// <summary>
        /// Gets a <see cref="UKKeyBind"/> given its name, if the keybind doesn't exist it will be created
        /// </summary>
        /// <param name="key">The name of the keybind</param>
        /// <param name="fallback">The default key of the keybind</param>
        /// <returns>An instance of a <see cref="UKKeyBind"/></returns>
        public static UKKeyBind GetKeyBind(string key, KeyCode fallback = KeyCode.None)
        {
            UKKeyBind bind = KeyBindHandler.GetKeyBind(key, fallback);
            if (!bind.enabled)
            {
                bind.enabled = true;
                KeyBindHandler.OnKeyBindEnabled.Invoke(bind);
            }
            return bind;
        }

        /// <summary>
        /// Ensures that a <see cref="UKKeyBind"/> exists given a key, if it doesn't exist it won't be created
        /// </summary>
        /// <param name="key">The name of the keybind</param>
        /// <returns>If the keybind exists</returns>
        public static bool EnsureKeyBindExists(string key)
        {
            return KeyBindHandler.moddedKeyBinds.ContainsKey(key);
        }

        /// <summary>
        /// Checks if a mod is loaded provided its GUID
        /// </summary>
        /// <param name="GUID">The GUID of the mod</param>
        /// <returns></returns>
        public static bool IsModLoaded(string GUID)
        {
            return UltraModManager.allLoadedMods.ContainsKey(GUID);
        }

        [Obsolete("Use AllModInfoClone instead.")]
        public static Dictionary<string, ModInformation> GetAllModInformation() => AllModInfoClone;

        [Obsolete("Use AllLoadedModInfoClone instead.")]
        public static Dictionary<string, ModInformation> GetAllLoadedModInformation() => AllLoadedModInfoClone;

        /// <summary>
        /// Restarts Ultrakill
        /// </summary>
        public static void Restart() // thanks https://gitlab.com/vtolvr-mods/ModLoader/-/blob/release/Launcher/Program.cs
        {
            Application.Quit();
            Plugin.logger.LogMessage("Restarting Ultrakill!");

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = @"steam://run/1229490",
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized
            };
            System.Diagnostics.Process.Start(psi);
        }
    }
}
