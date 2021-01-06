using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Caffeine
{
    class CursorObserver
    {
        private Thread _thread;
        private bool _keepRunning;
        private Point _lastPosition = new Point(0, 0);
        private int _samePositionCounter = 0;
        private readonly int _limitSamePositionCounter = 60;

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
                if (IsInSamePlaceForTooLong())
                    MoveAround();
            }
        }

        private bool IsInSamePlaceForTooLong()
        {
            var currerntPosition = Cursor.Position;
            bool samePlace = _lastPosition.X == currerntPosition.X && _lastPosition.Y == currerntPosition.Y;
            if (samePlace)
                _samePositionCounter++;
            else
                _samePositionCounter = 0;

            _lastPosition = currerntPosition;
            var ret = _samePositionCounter > _limitSamePositionCounter;
            Debug.WriteLine($"Mesmo lugar: {samePlace}, por muito tempo ({_samePositionCounter}/{_limitSamePositionCounter}): {ret}");
            return ret;
        }

        private void MoveAround()
        {
            Debug.WriteLine("Movendo cursor...");
            var position = Cursor.Position;
            for (int i = 0; i <= 5; i++)
            {
                position.Y++;
                position.X++;
                Cursor.Position = position;
                Thread.Sleep(10);
            }
            for (int i = 0; i <= 5; i++)
            {
                position.Y--;
                position.X--;
                Cursor.Position = position;
                Thread.Sleep(10);
            }
            for (int i = 0; i <= 5; i++)
            {
                position.Y--;
                position.X--;
                Cursor.Position = position;
                Thread.Sleep(10);
            }
            for (int i = 0; i <= 5; i++)
            {
                position.Y++;
                position.X++;
                Cursor.Position = position;
                Thread.Sleep(10);
            }

            Cursor.Position = _lastPosition; // volta onde estava
            _samePositionCounter = 0;
        }

        public void Stop()
        {
            _keepRunning = false;
            _thread.Abort();
        }
    }
}
