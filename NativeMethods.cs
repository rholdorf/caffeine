﻿using System;
using System.Runtime.InteropServices;

namespace Caffeine
{
    internal static class NativeMethods
    {
        /// <summary>
        /// The SendInput function synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        /// <param name="numberOfInputs">Number of structures in the Inputs array.</param>
        /// <param name="inputs">Pointer to an array of INPUT structures. Each structure 
        /// represents an event to be inserted into the keyboard or mouse input stream.</param>
        /// <param name="sizeOfInputStructure">Specifies the size, in bytes, of an INPUT structure. 
        /// If cbSize is not the size of an INPUT structure, the function fails.</param>
        /// <returns>The function returns the number of events that it successfully inserted 
        /// into the keyboard or mouse input stream. If the function returns zero, the input 
        /// was already blocked by another thread. To get extended error information, call 
        /// GetLastError.
        /// Microsoft Windows Vista. This function fails when it is blocked by User Interface 
        /// Privilege Isolation (UIPI). Note that neither GetLastError nor the return value 
        /// will indicate the failure was caused by UIPI blocking.</returns>
        /// <remarks>
        /// Microsoft Windows Vista. This function is subject to UIPI. Applications are permitted 
        /// to inject input only into applications that are at an equal or lesser integrity level.
        /// The SendInput function inserts the events in the INPUT structures serially into the 
        /// keyboard or mouse input stream. These events are not interspersed with other keyboard 
        /// or mouse input events inserted either by the user (with the keyboard or mouse) or by 
        /// calls to keybd_event, mouse_event, or other calls to SendInput.
        /// This function does not reset the keyboard's current state. Any keys that are already 
        /// pressed when the function is called might interfere with the events that this function 
        /// generates. To avoid this problem, check the keyboard's state with the GetAsyncKeyState 
        /// function and correct as necessary.
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

        public static int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }
    }

    internal struct INPUT
    {
        /// <summary>
        /// Specifies the type of the input event. This member can be one of the following values. 
        /// <see cref="InputType.Mouse"/> - The event is a mouse event. Use the mi structure of the union.
        /// <see cref="InputType.Keyboard"/> - The event is a keyboard event. Use the ki structure of the union.
        /// <see cref="InputType.Hardware"/> - Windows 95/98/Me: The event is from input hardware other than a keyboard or mouse. Use the hi structure of the union.
        /// </summary>
        public uint Type;

        /// <summary>
        /// The data structure that contains information about the simulated Mouse, Keyboard or Hardware event.
        /// </summary>
        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    /// <summary>
    /// The combined/overlayed structure that includes Mouse, Keyboard and Hardware Input message data (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct MOUSEKEYBDHARDWAREINPUT
    {
        /// <summary>
        /// The <see cref="MOUSEINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;

        /// <summary>
        /// The <see cref="KEYBDINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;

        /// <summary>
        /// The <see cref="HARDWAREINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
    }

    /// <summary>
    /// The MOUSEINPUT structure contains information about a simulated mouse event. (see: http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    /// <remarks>
    /// If the mouse has moved, indicated by MOUSEEVENTF_MOVE, dx and dy specify 
    /// information about that movement. The information is specified as absolute
    /// or relative integer values. 
    /// If MOUSEEVENTF_ABSOLUTE value is specified, dx and dy contain normalized 
    /// absolute coordinates between 0 and 65,535. The event procedure maps these 
    /// coordinates onto the display surface. Coordinate (0,0) maps onto the 
    /// upper-left corner of the display surface; coordinate (65535,65535) maps 
    /// onto the lower-right corner. In a multimonitor system, the coordinates 
    /// map to the primary monitor. 
    /// Windows 2000/XP: If MOUSEEVENTF_VIRTUALDESK is specified, the coordinates 
    /// map to the entire virtual desktop.
    /// If the MOUSEEVENTF_ABSOLUTE value is not specified, dx and dy specify 
    /// movement relative to the previous mouse event (the last reported position). 
    /// Positive values mean the mouse moved right (or down); negative values mean 
    /// the mouse moved left (or up). 
    /// Relative mouse motion is subject to the effects of the mouse speed and the 
    /// two-mouse threshold values. A user sets these three values with the Pointer 
    /// Speed slider of the Control Panel's Mouse Properties sheet. You can obtain 
    /// and set these values using the SystemParametersInfo function. 
    /// The system applies two tests to the specified relative mouse movement. If 
    /// the specified distance along either the x or y axis is greater than the 
    /// first mouse threshold value, and the mouse speed is not zero, the system 
    /// doubles the distance. If the specified distance along either the x or y 
    /// axis is greater than the second mouse threshold value, and the mouse speed 
    /// is equal to two, the system doubles the distance that resulted from 
    /// applying the first threshold test. It is thus possible for the system to 
    /// multiply specified relative mouse movement along the x or y axis by up to 
    /// four times.
    /// </remarks>
    internal struct MOUSEINPUT
    {
        /// <summary>
        /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the x coordinate of the mouse; relative data is specified as the number of pixels moved. 
        /// </summary>
        public int X;

        /// <summary>
        /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the y coordinate of the mouse; relative data is specified as the number of pixels moved. 
        /// </summary>
        public int Y;

        /// <summary>
        /// If dwFlags contains MOUSEEVENTF_WHEEL, then mouseData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user. One wheel click is defined as WHEEL_DELTA, which is 120. 
        /// Windows Vista: If dwFlags contains MOUSEEVENTF_HWHEEL, then dwData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated to the right; a negative value indicates that the wheel was rotated to the left. One wheel click is defined as WHEEL_DELTA, which is 120.
        /// Windows 2000/XP: IfdwFlags does not contain MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then mouseData should be zero. 
        /// If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were pressed or released. This value may be any combination of the following flags. 
        /// </summary>
        public uint MouseData;

        /// <summary>
        /// A set of bit flags that specify various aspects of mouse motion and button clicks. The bits in this member can be any reasonable combination of the following values. 
        /// The bit flags that specify mouse button status are set to indicate changes in status, not ongoing conditions. For example, if the left mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN is set when the left button is first pressed, but not for subsequent motions. Similarly, MOUSEEVENTF_LEFTUP is set only when the button is first released. 
        /// You cannot specify both the MOUSEEVENTF_WHEEL flag and either MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP flags simultaneously in the dwFlags parameter, because they both require use of the mouseData field. 
        /// </summary>
        public uint Flags;

        /// <summary>
        /// Time stamp for the event, in milliseconds. If this parameter is 0, the system will provide its own time stamp. 
        /// </summary>
        public uint Time;

        /// <summary>
        /// Specifies an additional value associated with the mouse event. An application calls GetMessageExtraInfo to obtain this extra information. 
        /// </summary>
        public IntPtr ExtraInfo;
    }

    /// <summary>
    /// The KEYBDINPUT structure contains information about a simulated keyboard event.  (see: http://msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    /// <remarks>
    /// Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard-input methodssuch as handwriting recognition or voice recognitionas if it were text input by using the KEYEVENTF_UNICODE flag. If KEYEVENTF_UNICODE is specified, SendInput sends a WM_KEYDOWN or WM_KEYUP message to the foreground thread's message queue with wParam equal to VK_PACKET. Once GetMessage or PeekMessage obtains this message, passing the message to TranslateMessage posts a WM_CHAR message with the Unicode character originally specified by wScan. This Unicode character will automatically be converted to the appropriate ANSI value if it is posted to an ANSI window.
    /// Windows 2000/XP: Set the KEYEVENTF_SCANCODE flag to define keyboard input in terms of the scan code. This is useful to simulate a physical keystroke regardless of which keyboard is currently being used. The virtual key value of a key may alter depending on the current keyboard layout or what other keys were pressed, but the scan code will always be the same.
    /// </remarks>
    internal struct KEYBDINPUT
    {
        /// <summary>
        /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. The Winuser.h header file provides macro definitions (VK_*) for each value. If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0. 
        /// </summary>
        public ushort KeyCode;

        /// <summary>
        /// Specifies a hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application. 
        /// </summary>
        public ushort Scan;

        /// <summary>
        /// Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
        /// KEYEVENTF_EXTENDEDKEY - If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).
        /// KEYEVENTF_KEYUP - If specified, the key is being released. If not specified, the key is being pressed.
        /// KEYEVENTF_SCANCODE - If specified, wScan identifies the key and wVk is ignored. 
        /// KEYEVENTF_UNICODE - Windows 2000/XP: If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section. 
        /// </summary>
        public uint Flags;

        /// <summary>
        /// Time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp. 
        /// </summary>
        public uint Time;

        /// <summary>
        /// Specifies an additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information. 
        /// </summary>
        public IntPtr ExtraInfo;
    }

    /// <summary>
    /// The HARDWAREINPUT structure contains information about a simulated message generated by an input device other than a keyboard or mouse.  (see: http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    internal struct HARDWAREINPUT
    {
        /// <summary>
        /// Value specifying the message generated by the input hardware. 
        /// </summary>
        public uint Msg;

        /// <summary>
        /// Specifies the low-order word of the lParam parameter for uMsg. 
        /// </summary>
        public ushort ParamL;

        /// <summary>
        /// Specifies the high-order word of the lParam parameter for uMsg. 
        /// </summary>
        public ushort ParamH;
    }

}
