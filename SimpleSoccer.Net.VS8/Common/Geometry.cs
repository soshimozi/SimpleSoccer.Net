using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    static class Geometry
    {
        public enum PlaneLocation
        {
            Behind, InFront, On
        }

        public const double MinPrecision = 1E-24;
        public const double Epsilon = Constants.Epsilon; /* smallest such that 1.0+DBL_EPSILON != 1.0 */

        private const double pi = Math.PI; // 3.14159;

        /// <summary>
        /// Calculates the distince to the first interecting point in a circle
        /// </summary>
        /// <param name="RayOrigin"></param>
        /// <param name="RayHeading"></param>
        /// <param name="CircleOrigin"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double GetRayCircleIntersect(Vector2D RayOrigin,
                                    Vector2D RayHeading,
                                    Vector2D CircleOrigin,
                                    double radius)
        {
            Vector2D ToCircle = CircleOrigin - RayOrigin;
            double length = ToCircle.Length;
            double v = ToCircle.GetDotProduct(RayHeading);
            double d = radius * radius - (length * length - v * v);

            // If there was no intersection, return -1
            if (d < 0.0) return (-1.0);

            // Return the distance to the [first] intersecting point
            return (v - Math.Sqrt(d));
        }

        /// <summary>
        /// Check if a there is an intersection in a circle
        /// </summary>
        /// <param name="RayOrigin"></param>
        /// <param name="RayHeading"></param>
        /// <param name="CircleOrigin"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool DoRayCircleIntersect(Vector2D RayOrigin,
                                     Vector2D RayHeading,
                                     Vector2D CircleOrigin,
                                     double radius)
        {

            Vector2D ToCircle = CircleOrigin - RayOrigin;
            double length = ToCircle.Length;
            double v = ToCircle.GetDotProduct(RayHeading);
            double d = radius * radius - (length * length - v * v);

            // If there was no intersection, return -1
            return (d < 0.0);
        }


        /// <summary>
        ///  Given a point P and a circle of radius R centered at C this function
        ///  determines the two points on the circle that intersect with the 
        ///  tangents from P to the circle. Returns false if P is within the circle.
        ///
        ///  thanks to Dave Eberly for this one.
        /// </summary>
        /// <param name="C"></param>
        /// <param name="R"></param>
        /// <param name="P"></param>
        /// <param name="T1"></param>
        /// <param name="T2"></param>
        /// <returns></returns>
        public static bool GetTangentPoints(Vector2D C, double R, Vector2D P, Vector2D T1, Vector2D T2)
        {
            Vector2D PmC = P - C;
            double SqrLen = PmC.LengthSquared;
            double RSqr = R * R;
            if (SqrLen <= RSqr)
            {
                // P is inside or on the circle
                return false;
            }

            double InvSqrLen = 1 / SqrLen;
            double Root = Math.Sqrt(Math.Abs(SqrLen - RSqr));

            T1.X = C.X + R * (R * PmC.X - PmC.Y * Root) * InvSqrLen;
            T1.Y = C.Y + R * (R * PmC.Y + PmC.X * Root) * InvSqrLen;
            T2.X = C.X + R * (R * PmC.X + PmC.Y * Root) * InvSqrLen;
            T2.Y = C.Y + R * (R * PmC.Y - PmC.X * Root) * InvSqrLen;

            return true;
        }

        /// <summary>
        /// Given a line segment AB and a point P, this function calculates the 
        /// perpendicular distance between them
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="P"></param>
        /// <returns></returns>
        public static double DistToLineSegment(Vector2D A,
                                        Vector2D B,
                                        Vector2D P)
        {
            //if the angle is obtuse between PA and AB is obtuse then the closest
            //vertex must be A
            double dotA = (P.X - A.X) * (B.X - A.X) + (P.Y - A.Y) * (B.Y - A.Y);

            if (dotA <= 0) return Vector2D.Vec2DDistance(A, P);

            //if the angle is obtuse between PB and AB is obtuse then the closest
            //vertex must be B
            double dotB = (P.X - B.X) * (A.X - B.X) + (P.Y - B.Y) * (A.Y - B.Y);

            if (dotB <= 0) return Vector2D.Vec2DDistance(B, P);

            //calculate the point along AB that is the closest to P
            Vector2D Point = A + ((B - A) * dotA) / (dotA + dotB);

            //calculate the distance P-Point
            return Vector2D.Vec2DDistance(P, Point);
        }

        /// <summary>
        /// As above, but avoiding sqrt
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="P"></param>
        /// <returns></returns>
        public static double DistToLineSegmentSq(Vector2D A,
                                         Vector2D B,
                                         Vector2D P)
        {
            //if the angle is obtuse between PA and AB is obtuse then the closest
            //vertex must be A
            double dotA = (P.X - A.X) * (B.X - A.X) + (P.Y - A.Y) * (B.Y - A.Y);

            if (dotA <= 0) return Vector2D.Vec2DDistanceSq(A, P);

            //if the angle is obtuse between PB and AB is obtuse then the closest
            //vertex must be B
            double dotB = (P.X - B.X) * (A.X - B.X) + (P.Y - B.Y) * (A.Y - B.Y);

            if (dotB <= 0) return Vector2D.Vec2DDistanceSq(B, P);

            //calculate the point along AB that is the closest to P
            Vector2D Point = A + ((B - A) * dotA) / (dotA + dotB);

            //calculate the distance P-Point
            return Vector2D.Vec2DDistanceSq(P, Point);
        }

        /// <summary>
        /// Calculate distance to the intersection point of a ray and a plane
        /// </summary>
        /// <param name="RayOrigin"></param>
        /// <param name="RayHeading"></param>
        /// <param name="PlanePoint"></param>
        /// <param name="PlaneNormal"></param>
        /// <returns></returns>
        public static double DistanceToRayPlaneIntersection(Vector2D RayOrigin,
                                             Vector2D RayHeading,
                                             Vector2D PlanePoint,  //any point on the plane
                                             Vector2D PlaneNormal)
        {

            double d = -PlaneNormal.GetDotProduct(PlanePoint);
            double numer = PlaneNormal.GetDotProduct(RayOrigin) + d;
            double denom = PlaneNormal.GetDotProduct(RayHeading);

            // normal is parallel to vector
            if ((denom < Epsilon) && (denom > -Epsilon))
            {
                return (-1.0);
            }

            return -(numer / denom);
        }

        /// <summary>
        /// Determine where on a plane a point lies
        /// </summary>
        /// <param name="point"></param>
        /// <param name="PointOnPlane"></param>
        /// <param name="PlaneNormal"></param>
        /// <returns></returns>
        public static PlaneLocation WhereIsPoint(Vector2D point,
                              Vector2D PointOnPlane, //any point on the plane
                              Vector2D PlaneNormal)
        {
            Vector2D dir = PointOnPlane - point;

            double d = dir.GetDotProduct(PlaneNormal);

            if (d < -MinPrecision)
            {
                return PlaneLocation.InFront;
            }

            else if (d > MinPrecision)
            {
                return PlaneLocation.Behind;
            }

            return PlaneLocation.On;
        }

        /// <summary>
        ///	Given 2 lines in 2D space AB, CD this returns true if an 
        ///	intersection occurs.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public static bool LineIntersection2D(Vector2D A,
                               Vector2D B,
                               Vector2D C,
                               Vector2D D)
        {
            double rTop = (A.Y - C.Y) * (D.X - C.X) - (A.X - C.X) * (D.Y - C.Y);
            double sTop = (A.Y - C.Y) * (B.X - A.X) - (A.X - C.X) * (B.Y - A.Y);

            double Bot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

            if (Math.Abs(Bot) <= MinPrecision)//parallel
            {
                return false;
            }

            double invBot = 1.0 / Bot;
            double r = rTop * invBot;
            double s = sTop * invBot;

            if ((r > 0) && (r < 1) && (s > 0) && (s < 1))
            {
                //lines intersect
                return true;
            }

            //lines do not intersect
            return false;
        }

        /// <summary>
        ///	Given 2 lines in 2D space AB, CD this returns true if an 
        ///	intersection occurs and sets dist to the distance the intersection
        ///  occurs along AB
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static bool LineIntersection2D(Vector2D A,
                                Vector2D B,
                                Vector2D C,
                                Vector2D D,
                                ref double dist)
        {

            double rTop = (A.Y - C.Y) * (D.X - C.X) - (A.X - C.X) * (D.Y - C.Y);
            double sTop = (A.Y - C.Y) * (B.X - A.X) - (A.X - C.X) * (B.Y - A.Y);

            double Bot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);


            if (Bot == 0)//parallel
            {
                if (Math.Abs(rTop) > Geometry.MinPrecision && Math.Abs(sTop) > Geometry.MinPrecision)
                {
                    return true;
                }
                return false;
            }

            double r = rTop / Bot;
            double s = sTop / Bot;

            if ((r > 0) && (r < 1) && (s > 0) && (s < 1))
            {
                dist = Vector2D.Vec2DDistance(A, B) * r;

                return true;
            }

            else
            {
                dist = 0;

                return false;
            }
        }

        /// <summary>
        ///	Given 2 lines in 2D space AB, CD this returns true if an 
        ///	intersection occurs and sets dist to the distance the intersection
        ///  occurs along AB. Also sets the 2d vector point to the point of
        ///  intersection
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <param name="dist"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool LineIntersection2D(Vector2D A,
                                       Vector2D B,
                                       Vector2D C,
                                       Vector2D D,
                                       ref double dist,
                                       ref Vector2D point)
        {

            double rTop = (A.Y - C.Y) * (D.X - C.X) - (A.X - C.X) * (D.Y - C.Y);
            double rBot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

            double sTop = (A.Y - C.Y) * (B.X - A.X) - (A.X - C.X) * (B.Y - A.Y);
            double sBot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

            if ((rBot == 0) || (sBot == 0))
            {
                //lines are parallel
                return false;
            }

            double r = rTop / rBot;
            double s = sTop / sBot;

            if ((r > 0) && (r < 1) && (s > 0) && (s < 1))
            {
                dist = Vector2D.Vec2DDistance(A, B) * r;

                point = A + r * (B - A);

                return true;
            }

            else
            {
                dist = 0;

                return false;
            }
        }

        /// <summary>
        ///  tests two polygons for intersection. *Does not check for enclosure*
        /// 
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <returns></returns>
        public static bool ObjectIntersection2D(List<Vector2D> object1,
                                         List<Vector2D> object2)
        {
            //test each line segment of object1 against each segment of object2
            for (int r = 0; r < object1.Count - 1; ++r)
            {
                for (int t = 0; t < object2.Count - 1; ++t)
                {
                    if (LineIntersection2D(object2[t],
                                           object2[t + 1],
                                           object1[r],
                                           object1[r + 1]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///  tests a line segment against a polygon for intersection
        ///  *Does not check for enclosure*
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="objectToCheck"></param>
        /// <returns></returns>
        public static bool SegmentObjectIntersection2D(Vector2D A,
                                         Vector2D B,
                                         List<Vector2D> objectToCheck)
        {
            //test AB against each segment of object
            for (int r = 0; r < objectToCheck.Count - 1; ++r)
            {
                if (LineIntersection2D(A, B, objectToCheck[r], objectToCheck[r + 1]))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        ///  Returns true if the two circles overlap
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="r1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool TwoCirclesOverlapped(double x1, double y1, double r1,
                                  double x2, double y2, double r2)
        {
            double DistBetweenCenters = Math.Sqrt((x1 - x2) * (x1 - x2) +
                                              (y1 - y2) * (y1 - y2));

            if ((DistBetweenCenters < (r1 + r2)) || (DistBetweenCenters < Math.Abs(r1 - r2)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Returns true if the two circles overlap
        /// 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="r1"></param>
        /// <param name="c2"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool TwoCirclesOverlapped(Vector2D c1, double r1,
                                  Vector2D c2, double r2)
        {
            double DistBetweenCenters = Math.Sqrt((c1.X - c2.X) * (c1.X - c2.X) +
                                              (c1.Y - c2.Y) * (c1.Y - c2.Y));

            if ((DistBetweenCenters < (r1 + r2)) || (DistBetweenCenters < Math.Abs(r1 - r2)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  returns true if one circle encloses the other
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="r1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool TwoCirclesEnclosed(double x1, double y1, double r1,
                                double x2, double y2, double r2)
        {
            double DistBetweenCenters = Math.Sqrt((x1 - x2) * (x1 - x2) +
                                              (y1 - y2) * (y1 - y2));

            if (DistBetweenCenters < Math.Abs(r1 - r2))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Given two circles this function calculates the intersection points
        ///  of any overlap.
        ///
        ///  returns false if no overlap found
        ///
        /// see http://astronomy.swin.edu.au/~pbourke/geometry/2circle/
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="r1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="r2"></param>
        /// <param name="p3X"></param>
        /// <param name="p3Y"></param>
        /// <param name="p4X"></param>
        /// <param name="p4Y"></param>
        /// <returns></returns>
        public static bool TwoCirclesIntersectionPoints(double x1, double y1, double r1,
                                          double x2, double y2, double r2,
                                          out double p3X, out double p3Y,
                                          out double p4X, out double p4Y)
        {
            p3X = p3Y = p4X = p4Y = 0.0;

            //first check to see if they overlap
            if (!TwoCirclesOverlapped(x1, y1, r1, x2, y2, r2))
            {
                return false;
            }

            //calculate the distance between the circle centers
            double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            //Now calculate the distance from the center of each circle to the center
            //of the line which connects the intersection points.
            double a = (r1 - r2 + (d * d)) / (2 * d);
            double b = (r2 - r1 + (d * d)) / (2 * d);


            //MAYBE A TEST FOR EXACT OVERLAP? 

            //calculate the point P2 which is the center of the line which 
            //connects the intersection points
            double p2X, p2Y;

            p2X = x1 + a * (x2 - x1) / d;
            p2Y = y1 + a * (y2 - y1) / d;

            //calculate first point
            double h1 = Math.Sqrt((r1 * r1) - (a * a));

            p3X = p2X - h1 * (y2 - y1) / d;
            p3Y = p2Y + h1 * (x2 - x1) / d;


            //calculate second point
            double h2 = Math.Sqrt((r2 * r2) - (a * a));

            p4X = p2X + h2 * (y2 - y1) / d;
            p4Y = p2Y - h2 * (x2 - x1) / d;

            return true;

        }

        /// <summary>
        ///  Tests to see if two circles overlap and if so calculates the area
        ///  defined by the union
        ///
        /// see http://mathforum.org/library/drmath/view/54785.html
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="r1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static double TwoCirclesIntersectionArea(double x1, double y1, double r1,
                                          double x2, double y2, double r2)
        {
            //first calculate the intersection points
            double iX1, iY1, iX2, iY2;

            if (!TwoCirclesIntersectionPoints(x1, y1, r1, x2, y2, r2, out iX1, out iY1, out iX2, out iY2))
            {
                return 0.0; //no overlap
            }

            //calculate the distance between the circle centers
            double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            //find the angles given that A and B are the two circle centers
            //and C and D are the intersection points
            double CBD = 2 * Math.Acos((r2 * r2 + d * d - r1 * r1) / (r2 * d * 2));

            double CAD = 2 * Math.Acos((r1 * r1 + d * d - r2 * r2) / (r1 * d * 2));


            //Then we find the segment of each of the circles cut off by the 
            //chord CD, by taking the area of the sector of the circle BCD and
            //subtracting the area of triangle BCD. Similarly we find the area
            //of the sector ACD and subtract the area of triangle ACD.

            double area = 0.5f * CBD * r2 * r2 - 0.5f * r2 * r2 * Math.Sin(CBD) +
                          0.5f * CAD * r1 * r1 - 0.5f * r1 * r1 * Math.Sin(CAD);

            return area;
        }

        /// <summary>
        ///  given the radius, calculates the area of a circle
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double CircleArea(double radius)
        {
            return pi * radius * radius;
        }


        /// <summary>
        ///  returns true if the point p is within the radius of the given circle
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="radius"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool PointInCircle(Vector2D Pos,
                                  double radius,
                                  Vector2D p)
        {
            double DistFromCenterSquared = ((Vector2D)(p - Pos)).LengthSquared;

            if (DistFromCenterSquared < (radius * radius))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  returns true if the line segemnt AB intersects with a circle at
        ///  position P with radius radius
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="P"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool LineSegmentCircleIntersection(Vector2D A,
                                                    Vector2D B,
                                                    Vector2D P,
                                                    double radius)
        {
            //first determine the distance from the center of the circle to
            //the line segment (working in distance squared space)
            double DistToLineSq = DistToLineSegmentSq(A, B, P);

            if (DistToLineSq < radius * radius)
            {
                return true;
            }

            else
            {
                return false;
            }

        }

        /// <summary>
        ///  given a line segment AB and a circle position and radius, this function
        ///  determines if there is an intersection and stores the position of the 
        ///  closest intersection in the reference IntersectionPoint
        ///
        ///  returns false if no intersection point is found
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="pos"></param>
        /// <param name="radius"></param>
        /// <param name="IntersectionPoint"></param>
        /// <returns></returns>
        public static bool GetLineSegmentCircleClosestIntersectionPoint(Vector2D A,
                                                                 Vector2D B,
                                                                 Vector2D pos,
                                                                 double radius,
                                                                 ref Vector2D IntersectionPoint)
        {
            Vector2D toBNorm = Vector2D.Vec2DNormalize(B - A);

            //move the circle into the local space defined by the vector B-A with origin
            //at A
            Vector2D LocalPos = Transformations.PointToLocalSpace(pos, toBNorm, toBNorm.Perp, A);

            bool ipFound = false;

            //if the local position + the radius is negative then the circle lays behind
            //point A so there is no intersection possible. If the local x pos minus the 
            //radius is greater than length A-B then the circle cannot intersect the 
            //line segment
            if ((LocalPos.X + radius >= 0) &&
               ((LocalPos.X - radius) * (LocalPos.X - radius) <= Vector2D.Vec2DDistanceSq(B, A)))
            {
                //if the distance from the x axis to the object's position is less
                //than its radius then there is a potential intersection.
                if (Math.Sqrt(LocalPos.Y) < radius)
                {
                    //now to do a line/circle intersection test. The center of the 
                    //circle is represented by A, B. The intersection points are 
                    //given by the formulae x = A +/-sqrt(r^2-B^2), y=0. We only 
                    //need to look at the smallest positive value of x.
                    double a = LocalPos.X;
                    double b = LocalPos.Y;

                    double ip = a - Math.Sqrt(radius * radius - b * b);

                    if (ip <= 0)
                    {
                        ip = a + Math.Sqrt(radius * radius - b * b);
                    }

                    ipFound = true;

                    IntersectionPoint = A + toBNorm * ip;
                }
            }

            return ipFound;
        }
    }
}
