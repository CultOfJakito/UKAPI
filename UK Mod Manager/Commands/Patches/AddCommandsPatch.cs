using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameConsole;
using HarmonyLib;
using Console = GameConsole.Console;

namespace UKAPI.Commands
{

	[HarmonyPatch(typeof(Console), "Awake")]
	static class AddCommandsPatch
	{
		private static void Postfix()
		{
			RegisterCommands(CommandRegistry.cmds);
		}

		private static void RegisterCommands(IEnumerable<ICommand> cmds)
		{
			foreach (ICommand cmd in cmds)
			{
				if (Console.Instance.recognizedCommands.ContainsKey(cmd.Command))
				{
					continue;
				}

				Console.Instance.RegisterCommand(cmd);
			}
		}
	}
}
