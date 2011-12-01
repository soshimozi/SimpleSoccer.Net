using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class Point2D
    {
        double _x;
        double _y;

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
    }
}
