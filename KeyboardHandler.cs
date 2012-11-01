// ////
// //// Jonathan Karlsson
// //// 2012-10-27
// ////

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace StarcraftRandom
{
	public class KeyboardHandler : IDisposable
	{
		#region public

		public KeyboardHandler(Window mainWindow, int keycode)
		{
			host = new WindowInteropHelper(mainWindow);
			this.keycode = keycode;

			SetupHotKey(host.Handle, this.keycode);
			ComponentDispatcher.ThreadPreprocessMessage += PreprocessMessage;
		}

		public void Dispose()
		{
			UnregisterHotKey(host.Handle, GetType().GetHashCode());
		}

		public event EventHandler KeyPressed;

		#endregion

		#region private

		private void SetupHotKey(IntPtr handle, int key)
		{
			RegisterHotKey(handle, GetType().GetHashCode(), 0, key);
		}

		private void PreprocessMessage(ref MSG msg, ref bool handled)
		{
			if (msg.message == WmHotkey)
			{
				//SetForegroundWindow(host.Handle);
				if (KeyPressed != null)
				{
					KeyPressed(this, new EventArgs());
				}
			}
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		[DllImport("User32.dll")]
		public static extern Int32 SetForegroundWindow(IntPtr hWnd);

		private const int WmHotkey = 0x0312;
		private readonly WindowInteropHelper host;
		private readonly int keycode;

		#endregion
	}
}