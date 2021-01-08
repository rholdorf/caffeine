using System.Diagnostics;
using System.Drawing;
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

        public void Start()
        {
            _keepRunning = true;
            _thread = new Thread(() => Observe()) { IsBackground = true };
            _thread.Start();
        }

        private void Observe()
        {
            while (_keepRunning)
            {
                Thread.Sleep(1000);
                //if (ScreenSaver.GetScreenSaverRunning())
                //{
                //    Debug.WriteLine("running, will kill");
                //    ScreenSaver.KillScreenSaver();
                //}
                if (IsInSamePlaceForTooLong())
                {
                    SimulateAction();
                }
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
            Debug.WriteLine("Simulando ação...");
            _inputSimulator.Mouse.MoveMouseBy(3, 3);
            Thread.Sleep(10);
            _inputSimulator.Mouse.MoveMouseBy(-3, -3);
            _idleCheckCounter = 0;
        }

        public void Stop()
        {
            _keepRunning = false;
            _thread.Abort();
        }
    }
}
