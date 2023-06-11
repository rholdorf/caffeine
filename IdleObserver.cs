using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;

namespace Caffeine
{
    class IdleObserver : IDisposable
    {
        private Thread _thread;
        private bool _keepRunning;
        private Point _lastPosition = new Point(0, 0);
        private int _idleCheckCounter = 0;
        private readonly int _limitSamePositionCounter = 59;
        private readonly InputSimulator _inputSimulator = new InputSimulator();
        private readonly NotifyIcon _trayIcon;
        private const string OBSERVANDO = "Observing";
        private const string SIMULANDO = "Simulating action...";
        private Random _random = new Random();
        private bool _systemAwake = true;
        private bool _disposed;

        public IdleObserver(NotifyIcon trayIcon)
        {
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            _trayIcon = trayIcon;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch(e.Mode)
            {
                case PowerModes.Resume:
                    _systemAwake = true;
                    Start();
                    break;

                case PowerModes.Suspend:
                    _systemAwake = false;
                    Stop();
                    break;
            }
        }

        public void Start()
        {
            _keepRunning = true;
            _thread = new Thread(() => Observe()) { IsBackground = true };
            _thread.Start();
        }

        private void SetTrayToolTip(string value)
        {
            _trayIcon.Text = value;
        }

        private void Observe()
        {
            try
            {
                SetTrayToolTip(OBSERVANDO);
                while (_keepRunning)
                {
                    Thread.Sleep(1000);
                    if (IsInSamePlaceForTooLong())
                        SimulateAction();
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                while (ex != null)
                {
                    sb.AppendLine(DateTime.Now.ToString("o"));
                    sb.AppendLine($"System awake: {_systemAwake}");
                    sb.AppendLine(ex.GetType().ToString());
                    sb.AppendLine(ex.Message);
                    sb.AppendLine(ex.StackTrace);
                    ex = ex.InnerException;
                }

                File.AppendAllText("exception.log", sb.ToString());
                throw;
            }
        }

        private bool IsInSamePlaceForTooLong()
        {
            var currerntPosition = Cursor.Position;
            bool samePlace = _lastPosition.X == currerntPosition.X && _lastPosition.Y == currerntPosition.Y;
            if (samePlace)
                _idleCheckCounter++;
            else
                _idleCheckCounter = 0;

            _lastPosition = currerntPosition;
            var ret = _idleCheckCounter > _limitSamePositionCounter;
            Debug.WriteLine($"Same spot: {samePlace}, for too long ({_idleCheckCounter}/{_limitSamePositionCounter}): {ret}");
            return ret;
        }

        private void SimulateAction()
        {
            if (!_systemAwake)
                return;

            var x = _random.Next(3, 7);
            var y = _random.Next(3, 7);
            var sleepMs = _random.Next(10, 100);

            try
            {
                Debug.WriteLine(SIMULANDO);
                SetTrayToolTip(SIMULANDO);
                _inputSimulator.Mouse.MoveMouseBy(x, y);
                Thread.Sleep(sleepMs);
                _inputSimulator.Mouse.MoveMouseBy(-x, -y);
            }
            catch(Exception ex)
            {
                // skip lib error
                if (ex.Message.Contains("Some simulated input commands were not sent successfully."))
                    Debug.WriteLine(ex.Message);
                else
                    throw;
            }

            _idleCheckCounter = 0;
            SetTrayToolTip(OBSERVANDO);
        }

        public void Stop()
        {
            _keepRunning = false;
            _thread?.Abort();
            _thread = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Because this is a static event, you must detach your event
                    // handlers when your application is disposed, or memory leaks
                    // will result.
                    SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
