using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SimpleSoccer.Net
{
    public class Vector2D
    {
        private Guid _id = Guid.NewGuid();

        private double _x;
        private double _y;

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

        public void Zero()
        {
            _x = _y = 0.0;
        }

        public bool IsZero
        {
            get { return ((_x * _x + _y * _y) < Geometry.MinPrecision); }
        }

        public PointF ToPoint()
        {
            return new PointF((float)_x, (float)_y);
        }

        public static Vector2D operator /(Vector2D lhs, Vector2D rhs)
        {
            Vector2D returnVal = new Vector2D();
            returnVal.X = lhs.X / rhs.X;
            returnVal.Y = lhs.Y / rhs.Y;
            return returnVal;
        }

        public static Vector2D operator /(Vector2D lhs, double rhs)
        {
            Vector2D returnVal = new Vector2D();
            returnVal.X = lhs.X / rhs;
            returnVal.Y = lhs.Y / rhs;
            return returnVal;
        }

        public static Vector2D operator /(double lhs, Vector2D rhs)
        {
            Vector2D returnVal = new Vector2D();
            returnVal.X = lhs / rhs.X;
            returnVal.Y = lhs / rhs.Y;
            return returnVal;
        }

        public static Vector2D operator *(Vector2D lhs, double rhs)
        {
            Vector2D returnVal = new Vector2D();
            returnVal.X = lhs.X * rhs;
            returnVal.Y = lhs.Y * rhs;
            return returnVal;
        }

        public static Vector2D operator *(double lhs, Vector2D rhs)
        {
            Vector2D returnVal = new Vector2D();
            returnVal.X = lhs * rhs.X;
            returnVal.Y = lhs * rhs.Y;
            return returnVal;
        }

        public static Vector2D operator *(Vector2D lhs, Vector2D rhs)
        {
            Vector2D returnVal = new Vector2D();
            returnVal.X = lhs.X * rhs.X;
            returnVal.Y = lhs.Y * rhs.Y;
            return returnVal;
        }

        public static Vector2D operator +(Vector2D lhs, Vector2D rhs)
        {
            Vector2D returnVal = new Vector2D();
            returnVal.X = lhs.X + rhs.X;
            returnVal.Y = lhs.Y + rhs.Y;
            return returnVal;
        }

        public static Vector2D operator -(Vector2D lhs, Vector2D rhs)
        {
            Vector2D returnVal = new Vector2D();
            returnVal.X = lhs.X - rhs.X;
            returnVal.Y = lhs.Y - rhs.Y;
            return returnVal;
        }

        //public override int GetHashCode()
        //{
        //    return _id.GetHashCode();
        //}

        //public override bool Equals(object obj)
        //{
        //    Vector2D rhs = obj as Vector2D;
        //    Vector2D lhs = this;
        //    return Math.Abs(lhs.X - rhs.X) < Geometry.MinPrecision && Math.Abs(lhs.Y - rhs.Y) < Geometry.MinPrecision;
        //}

        //public static bool operator ==(Vector2D lhs, Vector2D rhs)
        //{
        //    return Math.Abs(lhs.X - rhs.X) < Geometry.MinPrecision && Math.Abs(lhs.Y - rhs.Y) < Geometry.MinPrecision;
        //}

        //public static bool operator !=(Vector2D lhs, Vector2D rhs)
        //{
        //    return Math.Abs(lhs.X - rhs.X) > Geometry.MinPrecision || Math.Abs(lhs.Y - rhs.Y) > Geometry.MinPrecision;
        //}

        public double Length
        {
            get { return Math.Sqrt(_x * _x + _y * _y); }
        }

        public double LengthSquared
        {
            get { return (_x * _x + _y * _y); }
        }

        public double GetDotProduct(Vector2D vector)
        {
            return _x * vector.X + _y * vector.Y;
        }

        public int GetSign(Vector2D vector)
        {
            return (_y * vector._x > _x * vector._y) ? -1 : 1;
        }

        public Vector2D GetPerp()
        {
            Vector2D vector = new Vector2D();

            vector.X = _x;
            vector.Y = -_y;

            return vector;
        }

        public double GetDistance(Vector2D vector)
        {
            double ySeparation = vector._y - _y;
            double xSeparation = vector._x - _x;

            return Math.Sqrt(ySeparation * ySeparation + xSeparation * xSeparation);
        }

        public double GetDistanceSquared(Vector2D vector)
        {
            double ySeparation = vector._y - _y;
            double xSeparation = vector._x - _x;

            return (ySeparation * ySeparation + xSeparation * xSeparation);
        }

        public void Truncate(double maxLength)
        {
            if (Length > maxLength)
            {
                Normalize();

                _x *= maxLength;
                _y *= maxLength;
            }
        }

        public void Reflect(Vector2D normal)
        {
            Vector2D reflectiveVector = 2.0 * GetDotProduct(normal) * normal.GetReverse();

            _x += reflectiveVector.X;
            _y += reflectiveVector.Y;
        }

        public Vector2D GetReverse()
        {
            Vector2D vector = new Vector2D();

            vector.X = -_x;
            vector.Y = -_y;

            return vector;
        }

        public static Vector2D Vec2DNormalize(Vector2D vector)
        {
            Vector2D vectorOut = new Vector2D(vector.X, vector.Y);

            double length = vectorOut.Length;
            if (Math.Abs(length) > Geometry.MinPrecision)
            {
                vectorOut.X /= length;
                vectorOut.Y /= length;
            }

            return vectorOut;
        }

        public void Normalize()
        {
            double vector_length = Length;

            if (Math.Abs(vector_length) > Geometry.MinPrecision)
            {
                _x /= vector_length;
                _y /= vector_length;
            }
        }

        public Vector2D(Vector2D copy)
        {
            _x = copy.X;
            _y = copy.Y;
        }

        public Vector2D()
        {
        }

        public Vector2D(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public static double Vec2DDistance(Vector2D v1, Vector2D v2)
        {
            double ySeparation = v2.Y - v1.Y;
            double xSeparation = v2.X - v1.X;

            return Math.Sqrt(ySeparation * ySeparation + xSeparation * xSeparation);
        }

        public static double Vec2DDistanceSq(Vector2D v1, Vector2D v2)
        {
            double ySeparation = v2.Y - v1.Y;
            double xSeparation = v2.X - v1.X;

            return ySeparation * ySeparation + xSeparation * xSeparation;
        }

    }
}
