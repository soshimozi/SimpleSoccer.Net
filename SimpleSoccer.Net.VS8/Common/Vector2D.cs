using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    public class Vector2D
    {
        protected double x;
        protected double y;

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public static Vector2D operator *(double lhs, Vector2D rhs)
        {
            Vector2D result = new Vector2D(rhs);
            result.x *= lhs;
            result.y *= lhs;
            return result;
        }

        public static Vector2D operator *(Vector2D lhs, double rhs)
        {
            Vector2D result = new Vector2D(lhs);
            result.x *= rhs;
            result.y *= rhs;
            return result;
        }

        public static Vector2D operator +(Vector2D lhs, Vector2D rhs)
        {
            Vector2D result = new Vector2D(lhs);

            result.x += rhs.x;
            result.y += rhs.y;

            return result;
        }

        public static Vector2D operator -(Vector2D lhs, Vector2D rhs)
        {
            Vector2D result = new Vector2D(lhs);
            result.x -= rhs.x;
            result.y -= rhs.y;

            return result;
        }

        public static Vector2D operator /(Vector2D lhs, double val)
        {
            Vector2D result = new Vector2D(lhs);
            result.x /= val;
            result.y /= val;

            return result;
        }
    

        public Vector2D()
        {
            x = 0.0;
            y = 0.0;
        }

        public Vector2D(double a, double b)
        {
            x = a;
            y = b;
        }

        public Vector2D(Vector2D v2)
        {
            x = v2.x;
            y = v2.y;
        }

        public void Zero()
        {
            x = y = 0.0;
        }

        public bool IsZero
        {
            get
            {
                return (x * x + y * y) < Constants.Epsilon;
            }
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(x * x + y * y);
            }
        }

        public double LengthSquared
        {
            get
            {
                return x * x + y * y;
            }
        }

        public double GetDotProduct(Vector2D v2)
        {
            return x * v2.x + y * v2.y;
        }

        //public enum Direction 
        //{
        //    Clockwise = 1, 
        //    Anticlockwise = -1
        //}

        public int Sign(Vector2D v2)
        {
            if (y*v2.x > x*v2.y)
            { 
                return -1;
            }
            else 
            {
                return 1;
            }
        }

        public Vector2D Perp
        {
            get
            {
                return new Vector2D(-y, x);
            }
        }

        public double Distance(Vector2D v2)
        {
            double ySeparation = v2.y - y;
            double xSeparation = v2.x - x;

            return Math.Sqrt(ySeparation * ySeparation + xSeparation * xSeparation);
        }

        public double DistanceSq(Vector2D v2)
        {
            double ySeparation = v2.y - y;
            double xSeparation = v2.x - x;

            return ySeparation * ySeparation + xSeparation * xSeparation;
        }

        public void Truncate(double max)
        {
            if (Length > max)
            {
                Normalize();

                x *= max;
                y *= max;
            } 
        }

        public void Reflect(Vector2D normal)
        {
            Vector2D temp = new Vector2D(this);
            temp += 2.0 * GetDotProduct(normal) * normal.GetReverse();

            // copy calculated values over
            x = temp.x;
            y = temp.y;
        }

        public Vector2D GetReverse()
        {
            return new Vector2D(-x, -y);
        }

        public void Normalize()
        {
            double vector_length = this.Length;

            if (vector_length > Constants.Epsilon)
            {
                x /= vector_length;
                y /= vector_length;
            }
        }

        public static Vector2D Vec2DNormalize(Vector2D v)
        {
            Vector2D vec = new Vector2D(v);

            double vector_length = vec.Length;

            if (vector_length > Constants.Epsilon)
            {
                vec.x /= vector_length;
                vec.y /= vector_length;
            }

            return vec;
        }

        public static double Vec2DDistance(Vector2D v1, Vector2D v2)
        {
            double ySeparation = v2.y - v1.y;
            double xSeparation = v2.x - v1.x;

            return Math.Sqrt(ySeparation * ySeparation + xSeparation * xSeparation);
        }

        public static double Vec2DDistanceSq(Vector2D v1, Vector2D v2)
        {
            double ySeparation = v2.y - v1.y;
            double xSeparation = v2.x - v1.x;

            return (ySeparation * ySeparation + xSeparation * xSeparation);
        }

        public static double Vec2DLength(Vector2D v)
        {
            return Math.Sqrt(v.x * v.x + v.y * v.y);
        }

        public static double Vec2DLengthSq(Vector2D v)
        {
            return (v.x * v.x + v.y * v.y);
        }

        public static void WrapAround(Vector2D pos, int MaxX, int MaxY)
        {
            if (pos.x > MaxX) { pos.x = 0.0; }

            if (pos.x < 0) { pos.x = (double)MaxX; }

            if (pos.y < 0) { pos.y = (double)MaxY; }

            if (pos.y > MaxY) { pos.y = 0.0; }
        }

        public static bool NotInsideRegion(Vector2D p,
                                    Vector2D top_left,
                                    Vector2D bot_rgt)
        {
            return (p.x < top_left.x) || (p.x > bot_rgt.x) ||
                   (p.y < top_left.y) || (p.y > bot_rgt.y);
        }

        public static bool InsideRegion(Vector2D p,
                                    Vector2D top_left,
                                    Vector2D bot_rgt)
        {
            return !(p.x < top_left.x) || (p.x > bot_rgt.x) ||
                   (p.y < top_left.y) || (p.y > bot_rgt.y);
        }

        public static bool InsideRegion(Vector2D p, int left, int top, int right, int bottom)
        {
            return !((p.x < left) || (p.x > right) || (p.y < top) || (p.y > bottom));
        }

        //------------------ isSecondInFOVOfFirst -------------------------------------
        //
        //  returns true if the target position is in the field of view of the entity
        //  positioned at posFirst facing in facingFirst
        //-----------------------------------------------------------------------------
        public static bool isSecondInFOVOfFirst(Vector2D posFirst,
                                         Vector2D facingFirst,
                                         Vector2D posSecond,
                                         double fov)
        {
            Vector2D toTarget = Vec2DNormalize(posSecond - posFirst);

            return facingFirst.GetDotProduct(toTarget) >= Math.Cos(fov / 2.0);
        }

    }
}
