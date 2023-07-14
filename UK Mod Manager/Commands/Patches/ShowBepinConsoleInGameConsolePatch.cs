using BepInEx.Logging;
using GameConsole;
using HarmonyLib;
using Console = GameConsole.Console;

namespace UKAPI.Commands
{
    [HarmonyPatch(typeof(ManualLogSource), nameof(ManualLogSource.Log))]
    static class ShowBepinConsoleInGameConsolePatch
    {
        private static void Prefix(ManualLogSource __instance, LogLevel level, object data)
        {
            if (__instance.SourceName == "Unity Log")
            {
                return;
            }
            if (!ConvertLogLevels(level, out ConsoleLogType ukLevel))
            {
                return;
            }
            Console.Instance.PrintLine($"[{__instance.SourceName}] {data}", ukLevel);
        }

        private static bool ConvertLogLevels(LogLevel bepinLevel, out ConsoleLogType ukLevel)
        {
            switch (bepinLevel)
            {
                case LogLevel.Info:
                case LogLevel.Message:
                    ukLevel = ConsoleLogType.Log;
                    return true;
                case LogLevel.Warning:
                    ukLevel = ConsoleLogType.Warning;
                    return true;
                case LogLevel.Error:
                    ukLevel = ConsoleLogType.Error;
                    return true;
                default:
                    ukLevel = default;
                    return false;
            }
        }
    }
}
