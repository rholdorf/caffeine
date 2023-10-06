using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Caffeine
{
    internal class MouseMovementSimulator
    {
        private Random _random = new Random();

        public void Simulate()
        {
            var x = _random.Next(-7, 7);
            var y = _random.Next(-7, 7);
            var sleepMs = _random.Next(150, 300);

            try
            {
                Send(x, y);
                Thread.Sleep(sleepMs);
                Send(-x, -y);
            }
            catch (Exception ex)
            {
                SimpleLogger.Error(ex, $"x:{x} y:{y}");
                throw;
            }
        }

        private void Send(int x, int y)
        {
            var movement = new INPUT { Type = 0 /* mouse */ };
            movement.Data.Mouse.Flags = 1 /* move */;
            movement.Data.Mouse.X = x;
            movement.Data.Mouse.Y = y;
            var movementArray = new INPUT[] { movement };

            var result = NativeMethods.SendInput((UInt32)movementArray.Length, movementArray, Marshal.SizeOf(typeof(INPUT)));
            if (result != movementArray.Length)
            {
                var lastError = NativeMethods.GetLastError();
                SimpleLogger.Error(null, $"x:{x} y:{y} result:{result} lastError:{lastError}");
            }

            Debug.WriteLine($"x:{x} y:{y} result:{result}");
        }
    }
}
