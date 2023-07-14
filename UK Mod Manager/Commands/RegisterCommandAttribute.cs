using System;
using System.Collections.Generic;
using System.Text;

namespace UKAPI.Commands
{
	/// <summary>
	/// Used to recognize commands when using RegisterAllCommands
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class RegisterCommandAttribute : Attribute
	{
		public RegisterCommandAttribute()
		{

		}
	}
}
