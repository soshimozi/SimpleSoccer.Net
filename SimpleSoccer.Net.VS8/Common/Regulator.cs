using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    public class Regulator
    {
        private double _updatePeriod;
        private long _nextUpdateTick;
        private Random rng = new Random();
        
        public Regulator(double frequency)
        {
            // next update is a random time up to a second from now
            _nextUpdateTick = DateTime.Now.Ticks + (long)(rng.NextDouble() * TimeSpan.TicksPerSecond);

            if (frequency > 0)
            {
                _updatePeriod = TimeSpan.TicksPerSecond / frequency;
            }

            else if (Math.Abs(frequency) < Geometry.MinPrecision)
            {
                _updatePeriod = 0.0;
            }

            else if (frequency < 1)
            {
                _updatePeriod = -1;
            }
        }
        //the number of milliseconds the update period can vary per required
        //update-step. This is here to make sure any multiple clients of this class
        //have their updates spread evenly
        private const double UpdatePeriodVariator = 10.0;

        //returns true if the current time exceeds m_dwNextUpdateTime
        public bool IsReady
        {
            get
            {
                //if a regulator is instantiated with a zero freq then it goes into
                //stealth mode (doesn't regulate)
                if (Math.Abs(_updatePeriod) < Geometry.Epsilon) return true;

                //if the regulator is instantiated with a negative freq then it will
                //never allow the code to flow
                if (_updatePeriod < 0) return false;

                long currentTime = DateTime.Now.Ticks;


                if (currentTime >= _nextUpdateTick)
                {
                    _nextUpdateTick = (long)(currentTime + _updatePeriod + (-UpdatePeriodVariator + rng.NextDouble() * (UpdatePeriodVariator - -UpdatePeriodVariator)));
                    return true;
                }

                return false;
            }
        }
    }
}
