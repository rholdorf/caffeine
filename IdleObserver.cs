using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;

namespace Caffeine
{
    class IdleObserver
    {
        private Thread _thread;
        private bool _keepRunning;
        private Point _lastPosition = new Point(0, 0);
        private int _idleCheckCounter = 0;
        private readonly int _limitSamePositionCounter = 59;
        private readonly InputSimulator _inputSimulator = new InputSimulator();
        private readonly NotifyIcon _trayIcon;
        private const string OBSERVANDO = "Observando";
        private const string SIMULANDO = "Simulando ação...";
        private Random _random = new Random();

        public IdleObserver(NotifyIcon trayIcon)
        {
            _trayIcon = trayIcon;
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
            SetTrayToolTip(OBSERVANDO);
            while (_keepRunning)
            {
                Thread.Sleep(1000);
                if (IsInSamePlaceForTooLong())
                    SimulateAction();
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
            Debug.WriteLine($"Mesmo lugar: {samePlace}, por muito tempo ({_idleCheckCounter}/{_limitSamePositionCounter}): {ret}");
            return ret;
        }

        private void SimulateAction()
        {
            var x = _random.Next(3, 7);
            var y = _random.Next(3, 7);
            var sleepMs = _random.Next(10, 100);
            Debug.WriteLine("Simulando ação...");
            SetTrayToolTip(SIMULANDO);
            _inputSimulator.Mouse.MoveMouseBy(x, y);
            Thread.Sleep(sleepMs);
            _inputSimulator.Mouse.MoveMouseBy(-x, -y);
            _idleCheckCounter = 0;
            SetTrayToolTip(OBSERVANDO);
        }

        public void Stop()
        {
            _keepRunning = false;
            _thread.Abort();
        }
    }
}
