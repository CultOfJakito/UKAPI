using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;

namespace UKAPI.Internal
{
    [BepInPlugin("UKAPI", "ukapi.mainPlugin", VersionHandler.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static bool initialized = false;
        internal static Plugin instance;
        internal static ManualLogSource logger;

        private void Start()
        {
            if (!initialized)
            {
                instance = this;
                logger = Logger;
                logger.LogMessage("UKAPI initializing!");
                try
                {
                    new Harmony("ukapi.mainPlugin").PatchAll();

                    UKAPI.Initialize();
                    StartCoroutine(VersionHandler.CheckVersion());
                    initialized = true;
                }
                catch (ArgumentException e)
                {
                    logger.LogError("UMM failed to initialize");
                    logger.LogError(e.Message);
                    logger.LogError(e.StackTrace);
                }
            }
        }

        public void Update()
        {
            UKAPI.Update();
        }

        private void OnApplicationQuit()
        {
            SaveFileHandler.DumpFile();
        }
    }
}
