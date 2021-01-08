using System;
using System.Runtime.InteropServices;

namespace Caffeine
{
    public static class ScreenSaver
    {
        private const int SPI_GETSCREENSAVERACTIVE = 16;
        private const int SPI_SETSCREENSAVERACTIVE = 17;
        private const int SPI_GETSCREENSAVERTIMEOUT = 14;
        private const int SPI_GETBLOCKSENDINPUTRESETS = 0x1026;
        private const int SPI_SETSCREENSAVERTIMEOUT = 15;
        private const int SPI_GETSCREENSAVERRUNNING = 114;
        private const int SPIF_SENDWININICHANGE = 2;
        private const uint DESKTOP_WRITEOBJECTS = 0x0080;
        private const uint DESKTOP_READOBJECTS = 0x0001;
        private const int WM_CLOSE = 16;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(int uAction, int uParam, ref int lpvParam, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(int uAction, int uParam, ref bool lpvParam, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int PostMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr OpenDesktop(string hDesktop, int Flags, bool Inherit, uint DesiredAccess);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseDesktop(IntPtr hDesktop);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDesktopWindowsProc callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetForegroundWindow();

        private delegate bool EnumDesktopWindowsProc(IntPtr hDesktop, IntPtr lParam);

        /// <summary>
        /// Returns TRUE if the screen saver is active (enabled, but not necessarily running).
        /// </summary>
        /// <returns></returns>
        public static bool GetScreenSaverActive()
        {
            bool isActive = false;
            SystemParametersInfo(SPI_GETSCREENSAVERACTIVE, 0, ref isActive, 0);
            return isActive;
        }

        /// <summary>
        /// Pass in TRUE(1) to activate or FALSE(0) to deactivate the screen saver.
        /// </summary>
        /// <param name="Active"></param>
        public static void SetScreenSaverActive(int Active)
        {
            int nullVar = 0;
            SystemParametersInfo(SPI_SETSCREENSAVERACTIVE, Active, ref nullVar, SPIF_SENDWININICHANGE);
        }

        /// <summary>
        /// Returns the screen saver timeout setting, in seconds
        /// </summary>
        /// <returns></returns>
        public static int GetScreenSaverTimeout()
        {
            int value = 0;
            SystemParametersInfo(SPI_GETSCREENSAVERTIMEOUT, 0, ref value, 0);
            return value;
        }

        /// <summary>
        /// Retrieves a BOOL indicating whether an application can reset the screensaver's timer by calling the SendInput function to simulate keyboard or mouse input. 
        /// The pvParam parameter must point to a BOOL variable that receives TRUE if the simulated input will be blocked, or FALSE otherwise.
        /// </summary>
        /// <returns></returns>
        public static bool GetWillBlockSendInputReset()
        {
            bool value = false;
            SystemParametersInfo(SPI_GETBLOCKSENDINPUTRESETS, 0, ref value, 0);
            return value;
        }

        /// <summary>
        /// Pass in the number of seconds to set the screen saver timeout value.
        /// </summary>
        /// <param name="Value"></param>
        public static void SetScreenSaverTimeout(Int32 Value)
        {
            int nullVar = 0;
            SystemParametersInfo(SPI_SETSCREENSAVERTIMEOUT, Value, ref nullVar, SPIF_SENDWININICHANGE);
        }

        /// <summary>
        /// Returns TRUE if the screen saver is actually running
        /// </summary>
        /// <returns></returns>
        public static bool GetScreenSaverRunning()
        {
            bool isRunning = false;
            SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isRunning, 0);
            return isRunning;
        }

        public static void KillScreenSaver()
        {
            IntPtr hDesktop = OpenDesktop("Screen-saver", 0, false, DESKTOP_READOBJECTS | DESKTOP_WRITEOBJECTS);
            if (hDesktop != IntPtr.Zero)
            {
                EnumDesktopWindows(hDesktop, new EnumDesktopWindowsProc(KillScreenSaverFunc), IntPtr.Zero);
                CloseDesktop(hDesktop);
            }
            else
            {
                PostMessage(GetForegroundWindow(), WM_CLOSE, 0, 0);
            }
        }

        private static bool KillScreenSaverFunc(IntPtr hWnd, IntPtr lParam)
        {
            if (IsWindowVisible(hWnd))
                PostMessage(hWnd, WM_CLOSE, 0, 0);
            return true;
        }
    }
}
