using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using UKAPI.Commands;
using UnityEngine;

namespace UKAPI.Cheats
{
    public static class CheatRegistry
    {
			internal static List<CheatBase> cht = new List<CheatBase>();
     
     		/// <summary>
     		/// Register a cheat
     		/// </summary>
     		/// <param name="cheat">A cheat (Can inherit from CheatBase)</param>
     		public static void RegisterCheat(CheatBase cheat)
     		{
     			cht.Add(cheat);
                /*
	            if (cheat.CheatCategory != null)
	            {
		                CheatsManager.Instance.RegisterCheat(cheat, cheat.CheatCategory);

		                if (cheat.CheatIcon == null) return;
		                CheatsManager.Instance.spriteIcons.Add(cheat.Icon, cheat.CheatIcon);
		                return;
	            }
	            if (cheat.CheatIcon != null)
	            {
		            CheatsManager.Instance.spriteIcons.Add(cheat.Icon, cheat.CheatIcon);
		            CheatsManager.Instance.RegisterCheat(cheat);
	            }
	            else CheatsManager.Instance.RegisterCheat(cheat);
	            */
            }
     
     		/// <summary>
     		/// Registers a list of cheats
     		/// </summary>
     		/// <param name="commands">A list of cheats (each cheat can inherit from CheatBase or Iheat)</param>
     		public static void RegisterCheats(IEnumerable<CheatBase> cheats)
     		{
     			foreach (CheatBase cheat in cheats)
     			{
     				RegisterCheat(cheat);
     			}
     		}
     
     		/// <summary>
     		/// Registers all cheats in an assembly, checks for [RegisterCheat] attribute for every type/class
     		/// </summary>
     		/// <param name="asm"></param>
     		public static void RegisterAllCommands(Assembly asm)
     		{
     			RegisterCheats(asm.GetTypes()
     				.Where(IsPossibleCheat)
     				.Select(type => (CheatBase)Activator.CreateInstance(type)));
     		}
     
     		private static bool IsPossibleCheat(Type type)
     		{
     			if (type.IsInterface)
     			{
     				return false;
     			}
     
     			if (type.IsAbstract)
     			{
     				return false;
     			}
     
     			//Plugin.logger.LogDebug(type);
     
     			return type.GetCustomAttribute<RegisterCheatAttribute>() != null;
     		}   
    }
}