using System;
using System.Collections.Generic;
using System.Text;

namespace UKAPI.Commands
{

	[RegisterCommand]
	public sealed class TestCommand : CommandBase
	{
		public override string Description => "Example Command";

		/// <summary>
		/// Prefix for the command name, this command would be test_test (TestCommand becomes _test)
		/// </summary>
		protected override string Prefix => "test_";

		/// <summary>
		/// Behaviour of your code
		/// </summary>
		/// <param name="con">Game Console</param>
		/// <param name="args">list of arguments for your command</param>
		public override void Execute(GameConsole.Console con, string[] args)
		{
			con.PrintLine("Hello, World! UKAPI Here");
		}
	}
}
