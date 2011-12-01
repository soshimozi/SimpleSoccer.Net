using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SimpleSoccer.Net
{
    public class Region
    {
        public enum RegionModifier
        {
            Normal, 
            Halfsize
        }

        private double _top;
        private double _left;
        private double _right;
        private double _bottom;
        private int _regionId;

        private static Random rng = new Random();

        public double Top
        {
            get { return _top; }
            set { _top = value; }
        }

        public double Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public double Right
        {
            get { return _right; }
            set { _right = value; }
        }

        public double Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }

        public double Width
        {
            get { return Math.Abs(_right - _left); }
        }

        public double Height
        {
            get { return Math.Abs(_bottom - _top); }
        }

        public Vector2D VectorCenter
        {
            get { return new Vector2D((_left + _right) / 2.0, (_top + _bottom) / 2.0); }
        }

        public int RegionId
        {
            get { return _regionId; }
            set { _regionId = value; }
        }

        public Region()
        {
            _top = _bottom = _left = _right = 0;
            _regionId = -1;
        }

        public Region(double top, double bottom, double left, double right)
        {
            _top = top;
            _bottom = bottom;
            _left = left;
            _right = right;

            _regionId = -1;
        }

        public virtual void Render(bool showId, Graphics g)
        {
            Pen regionPen = new Pen(Color.Green, 1.0f);
            g.DrawRectangle(regionPen, (float)_left, (float)_top, (float)Width, (float)Height);
            if (showId)
            {
                g.DrawString(_regionId.ToString(), GDI.TextFont, Brushes.Green, new PointF((float)VectorCenter.X, (float)VectorCenter.Y));
            }
        }

        public Vector2D GetRandomPosition()
        {
            return new Vector2D(_left + rng.NextDouble()*Width, _top + rng.NextDouble()*Height);
        }

        public bool IsPositionInside(Vector2D position)
        {
            return IsPositionInside(position, RegionModifier.Normal);
        }

        public bool IsPositionInside(Vector2D position, RegionModifier modifier)
        {
            if (modifier == RegionModifier.Normal)
            {
                return ((position.X > _left) && (position.X < _right) &&
                     (position.Y > _top) && (position.Y < _bottom));
            }
            else
            {
                double marginX = Width * 0.25;
                double marginY = Height * 0.25;

                return ((position.X > (_left + marginX)) && (position.X < (_right - marginX)) &&
                     (position.Y > (_top + marginY)) && (position.Y < (_bottom - marginY)));
            }
        }
    }
}
