using System.Collections.Generic;
using HarmonyLib;

namespace UKAPI.Cheats.Patches
{
    [HarmonyPatch(typeof(CheatsManager), nameof(CheatsManager.Start))]
    public static class AddCheatsPatch
    {
        public static void Postfix(CheatsManager __instance)
        {
            foreach (CheatBase cht in CheatRegistry.cht)
            {
                if (__instance.idToCheat.ContainsKey(cht.Identifier))
                {
                    continue;
                }


                if (cht.CheatCategory != null)
                {
                    __instance.RegisterCheat(cht, cht.CheatCategory);
                }

                if (cht.CheatIcon != null)
                {
                    __instance.spriteIcons.Add(cht.Icon, cht.CheatIcon);
                }
                else __instance.RegisterCheat(cht);
                
            }
        }
    }
}