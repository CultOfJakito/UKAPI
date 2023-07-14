using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Logging;
using GameConsole;

namespace UKAPI.Commands
{
	public abstract class CommandBase : ICommand
	{
		public string Name => Command;

		public abstract string Description { get; }

		public virtual string Command { get; }

		protected virtual string Prefix => "";

		protected virtual string TypeNameForNameGeneration
		{
			get
			{
				string name = GetType().Name;
				if (name.EndsWith("Command"))
				{
					name = name.Substring(0, name.Length - "Command".Length);
				}

				return name;
			}
		}

		public abstract void Execute(GameConsole.Console con, string[] args);

		public CommandBase()
		{
			string name = TypeNameForNameGeneration;
			Command = Prefix + name.Replace(@"(?<!^)([A-Z])", "_$1").ToLower();
		}
	}
}
