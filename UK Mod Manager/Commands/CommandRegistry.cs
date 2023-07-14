using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GameConsole;

namespace UKAPI.Commands
{

	public static class CommandRegistry
	{
		internal static List<ICommand> cmds = new List<ICommand>();

		/// <summary>
		/// Register a Command
		/// </summary>
		/// <param name="command">A command (Can inherit from CommandBase or ICommand)</param>
		public static void RegisterCommand(ICommand command)
		{
			cmds.Add(command);
			GameConsole.Console.Instance?.RegisterCommand(command);
		}

		/// <summary>
		/// Registers a list of commands
		/// </summary>
		/// <param name="commands">A list of commands (each command can inherit from CommandBase or ICommand)</param>
		public static void RegisterCommands(IEnumerable<ICommand> commands)
		{
			foreach (ICommand command in commands)
			{
				RegisterCommand(command);
			}
		}

		/// <summary>
		/// Registers all commands in an assembly, checks for [RegisterCommand] attribute for every type/class
		/// </summary>
		/// <param name="asm"></param>
		public static void RegisterAllCommands(Assembly asm)
		{
			RegisterCommands(asm.GetTypes()
				.Where(IsPossibleCommand)
				.Select(type => (ICommand)Activator.CreateInstance(type)));
		}

		private static bool IsPossibleCommand(Type type)
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

			return type.GetCustomAttribute<RegisterCommandAttribute>() != null;
		}
	}
}
