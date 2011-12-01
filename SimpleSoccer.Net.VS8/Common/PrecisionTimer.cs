using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

namespace SimpleSoccer.Net
{

    class QueryPerformanceWrapper
    {
        [DllImport("Kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool QueryPerformanceCounter(out long
        lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool QueryPerformanceFrequency(out long
        lpFrequency);
    }

    public class PrecisionTimer
    {
        private const double Smoothness = 5.0;

        private long _currentTime = 0, 
                        _lastTime = 0,
                        _lastTimeInTimeElapsed = 0,
                        _nextTime = 0,
                        _startTime = 0,
                        _frameTime = 0,
                        _perfCountFreq = 0;

        private double _timeElapsed = 0.0,
                        _lastTimeElapsed = 0.0,
                        _timeScale = 0.0;

        private double _normalFPS = 0.0;
        private double _slowFPS = 1.0;

        private bool _started = false;

        //if true a call to TimeElapsed() will return 0 if the current
        //time elapsed is much smaller than the previous. Used to counter
        //the problems associated with the user using menus/resizing/moving 
        //a window etc
        private bool _smoothUpdates = false;


        public PrecisionTimer()
        {
            //how many ticks per sec do we get
            QueryPerformanceWrapper.QueryPerformanceFrequency(out _perfCountFreq);

            _timeScale = 1.0 / _perfCountFreq;
        }

        public PrecisionTimer(double fps)
        {
            _normalFPS = fps;

            //how many ticks per sec do we get
            QueryPerformanceWrapper.QueryPerformanceFrequency(out _perfCountFreq);

            _timeScale = 1.0 / _perfCountFreq;

            //calculate ticks per frame
            _frameTime = (long)(_perfCountFreq / _normalFPS);
        }

        public void Start()
        {
            _started = true;

            _timeElapsed = 0.0;

            //get the time
            QueryPerformanceWrapper.QueryPerformanceCounter(out _lastTime);

            //keep a record of when the timer was started
            _startTime = _lastTimeInTimeElapsed = _lastTime;

            //update time to render next frame
            _nextTime = _lastTime + _frameTime;
        }

        public bool ReadyForNextFrame()
        {
            Debug.Assert(Math.Abs(_normalFPS) > double.MinValue, "PrecisionTimer::ReadyForNextFrame<No FPS set in timer>");

            QueryPerformanceWrapper.QueryPerformanceCounter(out _currentTime);

            if (_currentTime > _nextTime)
            {

                _timeElapsed = (_currentTime - _lastTime) * _timeScale;
                _lastTime = _currentTime;

                //update time to render next frame
                _nextTime = _currentTime + _frameTime;

                return true;
            }

            return false;
        }

        //only use this after a call to ReadyForNextFrame.
        public double TimeElapsed()
        {
            _lastTimeElapsed = _timeElapsed;


            QueryPerformanceWrapper.QueryPerformanceCounter(out _currentTime);

            _timeElapsed = (_currentTime - _lastTimeInTimeElapsed) * _timeScale;

            _lastTimeInTimeElapsed = _currentTime;


            if (_smoothUpdates)
            {
                if (_timeElapsed < (_lastTimeElapsed * Smoothness))
                {
                    return _timeElapsed;
                }

                else
                {
                    return 0.0;
                }
            }

            else
            {
                return _timeElapsed;
            }
        }

        public double CurrentTime
        {
            get
            {
                QueryPerformanceWrapper.QueryPerformanceCounter(out _currentTime);
                return (_currentTime - _startTime) * _timeScale;
            }
        }

        public bool Started
        {
            get
            {
                return _started;
            }
        }

        public bool SmoothUpdates
        {
            get { return _smoothUpdates; }
            set { _smoothUpdates = value; }
        }
    }
}
