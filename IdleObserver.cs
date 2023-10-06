using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows.Forms;

namespace Caffeine
{
    class IdleObserver
    {
        private Thread _thread;
        private bool _keepRunning;
        private Point _lastPosition = new Point(0, 0);
        private DateTime _samePositionSince;
        private TimeSpan _timeThreshold = TimeSpan.FromMinutes(3);
        private readonly NotifyIcon _trayIcon;
        private const string OBSERVING = "Observing";
        private readonly MouseMovementSimulator _mouseMovementSimulator = new MouseMovementSimulator();
        private bool _disposed;

        public IdleObserver(NotifyIcon trayIcon)
        {
            _trayIcon = trayIcon;
            _trayIcon.Text = OBSERVING;
        }

        public void Start()
        {
            _keepRunning = true;
            _thread = new Thread(() => Observe()) { IsBackground = true };
            _thread.Start();
        }

        private void Observe()
        {
            try
            {
                while (_keepRunning)
                {
                    Thread.Sleep(5000);
                    if (IsInSamePlaceForTooLong())
                        _mouseMovementSimulator.Simulate();
                }
            }
            catch (Exception ex)
            {
                SimpleLogger.Error(ex);
                throw;
            }
        }

        private bool IsInSamePlaceForTooLong()
        {
            var currerntPosition = Cursor.Position;
            bool samePlace = _lastPosition.X == currerntPosition.X && _lastPosition.Y == currerntPosition.Y;
            _lastPosition = currerntPosition;

            if (!samePlace)
            {
                _samePositionSince = DateTime.MinValue;
                Debug.WriteLine("not yet");
                return false;
            }

            if (_samePositionSince == DateTime.MinValue)
            {
                _samePositionSince = DateTime.Now;
                Debug.WriteLine("same place!");
            }

            var howLong = DateTime.Now.Subtract(_samePositionSince);
            var tooLong = howLong > _timeThreshold;
            Debug.WriteLine($"same position for {howLong}. toolong: {tooLong}");
            return tooLong;
        }


        public void Stop()
        {
            _keepRunning = false;
            _thread?.Abort();
            _thread = null;
        }

    }
}
