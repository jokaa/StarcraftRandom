// ////
// //// Jonathan Karlsson
// //// 2012-10-27
// ////

using System.Windows.Input;

namespace StarcraftRandom
{
	internal static class CustomCommands
	{
		static CustomCommands()
		{
			exitCommand = new RoutedCommand("Exit", typeof (CustomCommands));
		}

		public static RoutedCommand Exit
		{
			get { return (exitCommand); }
		}

		private static readonly RoutedCommand exitCommand;
	}
}