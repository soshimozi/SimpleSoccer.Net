using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SimpleSoccer.Net
{
    static class Transformations
    {
        /// <summary>
        ///  given a List of 2D vectors, a position, orientation and scale,
        ///  this function transforms the 2D vectors into the object's world space
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="pos"></param>
        /// <param name="forward"></param>
        /// <param name="side"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static List<Vector2D> WorldTransform(List<Vector2D> points,
                                                    Vector2D pos,
                                                    Vector2D forward,
                                                    Vector2D side,
                                                    Vector2D scale)
        {
            //copy the original vertices into the buffer about to be transformed
            List<Vector2D> TranVector2Ds = new List<Vector2D>();
            
            // make deep copy of buffer
            for (int pointIndex = 0; pointIndex < points.Count; pointIndex++)
            {
                TranVector2Ds.Add(new Vector2D(points[pointIndex]));
            }

            //create a transformation matrix
            Matrix2D matTransform = new Matrix2D();

            //scale
            if ((scale.X != 1.0) || (scale.Y != 1.0))
            {
                matTransform.Scale((float)scale.X, (float)scale.Y);
            }

            //rotate
            matTransform.Rotate(forward, side);

            //matTransform.Rotate(190.0 * (3.14159 / 180));

            //and translate
            matTransform.Translate(pos.X, pos.Y);

            //now transform the object's vertices
            matTransform.TransformVector2Ds(TranVector2Ds);

            return TranVector2Ds;
        }

        /// <summary>
        ///  given a List of 2D vectors, a position and  orientation
        ///  this function transforms the 2D vectors into the object's world space
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="pos"></param>
        /// <param name="forward"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public static List<Vector2D> WorldTransform(List<Vector2D> points,
                                         Vector2D pos,
                                         Vector2D forward,
                                         Vector2D side)
        {
            //copy the original vertices into the buffer about to be transformed
            List<Vector2D> TranVector2Ds = new List<Vector2D>();

            // make deep copy of buffer
            for (int pointIndex = 0; pointIndex < points.Count; pointIndex++)
            {
                TranVector2Ds.Add(new Vector2D(points[pointIndex]));
            }

            //create a transformation matrix
            Matrix2D matTransform = new Matrix2D();

            //rotate
            matTransform.Rotate(forward, side);

            //and translate
            matTransform.Translate(pos.X, pos.Y);

            //now transform the object's vertices
            matTransform.TransformVector2Ds(TranVector2Ds);

            return TranVector2Ds;
        }

        /// <summary>
        ///  Transforms a point from the agent's local space into world space
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="AgentHeading"></param>
        /// <param name="AgentSide"></param>
        /// <param name="AgentPosition"></param>
        /// <returns></returns>
        public static Vector2D PointToWorldSpace(Vector2D point,
                                            Vector2D AgentHeading,
                                            Vector2D AgentSide,
                                            Vector2D AgentPosition)
        {
            //make a copy of the point
            Vector2D TransPoint = new Vector2D(point);

            //create a transformation matrix
            Matrix2D matTransform = new Matrix2D();

            //rotate
            //matTransform.Rotate(AgentHeading, AgentSide);

            //and translate
            matTransform.Translate(AgentPosition.X, AgentPosition.Y);

            //now transform the vertices
            matTransform.TransformVector2Ds(TransPoint);

            return TransPoint;
        }

        /// <summary>
        ///  Transforms a vector from the agent's local space into world space
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="AgentHeading"></param>
        /// <param name="AgentSide"></param>
        /// <returns></returns>
        public static Vector2D VectorToWorldSpace(Vector2D vec,
                                             Vector2D AgentHeading,
                                             Vector2D AgentSide)
        {
            //make a copy of the point
            Vector2D TransVec = new Vector2D(vec);

            //create a transformation matrix
            Matrix2D matTransform = new Matrix2D();

            //rotate
            //matTransform.Rotate(AgentHeading, AgentSide);

            //now transform the vertices
            matTransform.TransformVector2Ds(TransVec);

            return TransVec;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="AgentHeading"></param>
        /// <param name="AgentSide"></param>
        /// <param name="AgentPosition"></param>
        /// <returns></returns>
        public static Vector2D PointToLocalSpace(Vector2D point,
                                     Vector2D AgentHeading,
                                     Vector2D AgentSide,
                                      Vector2D AgentPosition)
        {

            //make a copy of the point
            Vector2D TransPoint = new Vector2D(point);

            //create a transformation matrix
            Matrix2D matTransform = new Matrix2D();

            double Tx = -AgentPosition.GetDotProduct(AgentHeading);
            double Ty = -AgentPosition.GetDotProduct(AgentSide);

            //create the transformation matrix
            matTransform._11 = AgentHeading.X; matTransform._12 = AgentSide.X;
            matTransform._21 = AgentHeading.Y; matTransform._22 = AgentSide.Y;
            matTransform._31 = Tx; matTransform._32 = Ty;
            //matTransform._11( AgentHeading.X ); matTransform._12( AgentSide.X );
            //matTransform._21( AgentHeading.Y ); matTransform._22( AgentSide.Y );
            //matTransform._31( Tx ); matTransform._32( Ty );

            ////now transform the vertices
            matTransform.TransformVector2Ds(TransPoint);

            return TransPoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="AgentHeading"></param>
        /// <param name="AgentSide"></param>
        /// <returns></returns>
        //public static Vector2D VectorToLocalSpace(Vector2D vec,
        //                             Vector2D AgentHeading,
        //                             Vector2D AgentSide)
        //{

        //    //make a copy of the point
        //    Vector2D TransPoint = new Vector2D(vec);

        //    //create a transformation matrix
        //    Matrix2D matTransform = new Matrix2D();

        //    //create the transformation matrix
        //    matTransform._11 = AgentHeading.X; matTransform._12 = AgentSide.X;
        //    matTransform._21 = (AgentHeading.Y); matTransform._22 = (AgentSide.Y);

        //    //now transform the vertices
        //    matTransform.TransformVector2Ds(TransPoint);

        //    return TransPoint;
        //}

        /// <summary>
        ///  rotates a vector ang rads around the origin
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="ang"></param>
        public static void Vec2DRotateAroundOrigin(Vector2D v, double ang)
        {
            //create a transformation matrix
            Matrix2D mat = new Matrix2D();

            //rotate
            mat.Rotate(ang);

            //now transform the object's vertices
            mat.TransformVector2Ds(v);
        }

        /// <summary>
        ///  given an origin, a facing direction, a 'field of view' describing the 
        ///  limit of the outer whiskers, a whisker length and the number of whiskers
        ///  this method returns a vector containing the end positions of a series
        ///  of whiskers radiating away from the origin and with equal distance between
        ///  them. (like the spokes of a wheel clipped to a specific segment size)
        /// 
        /// </summary>
        /// <param name="NumWhiskers"></param>
        /// <param name="WhiskerLength"></param>
        /// <param name="fov"></param>
        /// <param name="facing"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static List<Vector2D> CreateWhiskers(int NumWhiskers,
                                                    double WhiskerLength,
                                                    double fov,
                                                    Vector2D facing,
                                                    Vector2D origin)
        {
            //this is the magnitude of the angle separating each whisker
            double SectorSize = fov / (double)(NumWhiskers - 1);

            List<Vector2D> whiskers = new List<Vector2D>();
            Vector2D temp;
            double angle = -fov * 0.5;

            for (int w = 0; w < NumWhiskers; ++w)
            {
                //create the whisker extending outwards at this angle
                temp = new Vector2D(facing);
                Vec2DRotateAroundOrigin(temp, angle);
                whiskers.Add(origin + WhiskerLength * temp);

                angle += SectorSize;
            }

            return whiskers;
        }
    }
}
