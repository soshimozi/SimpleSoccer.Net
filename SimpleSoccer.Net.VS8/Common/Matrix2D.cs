using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class Matrix2D
    {
        struct Matrix
        {

            public double _11, _12, _13;
            public double _21, _22, _23;
            public double _31, _32, _33;

        };

        Matrix _matrix = new Matrix();

        void MatrixMultiply(Matrix mIn)
        {
            Matrix mat_temp = new Matrix();

            //first row
            mat_temp._11 = (_matrix._11 * mIn._11) + (_matrix._12 * mIn._21) + (_matrix._13 * mIn._31);
            mat_temp._12 = (_matrix._11 * mIn._12) + (_matrix._12 * mIn._22) + (_matrix._13 * mIn._32);
            mat_temp._13 = (_matrix._11 * mIn._13) + (_matrix._12 * mIn._23) + (_matrix._13 * mIn._33);

            //second
            mat_temp._21 = (_matrix._21 * mIn._11) + (_matrix._22 * mIn._21) + (_matrix._23 * mIn._31);
            mat_temp._22 = (_matrix._21 * mIn._12) + (_matrix._22 * mIn._22) + (_matrix._23 * mIn._32);
            mat_temp._23 = (_matrix._21 * mIn._13) + (_matrix._22 * mIn._23) + (_matrix._23 * mIn._33);

            //third
            mat_temp._31 = (_matrix._31 * mIn._11) + (_matrix._32 * mIn._21) + (_matrix._33 * mIn._31);
            mat_temp._32 = (_matrix._31 * mIn._12) + (_matrix._32 * mIn._22) + (_matrix._33 * mIn._32);
            mat_temp._33 = (_matrix._31 * mIn._13) + (_matrix._32 * mIn._23) + (_matrix._33 * mIn._33);

            _matrix._11 = mat_temp._11;
            _matrix._12 = mat_temp._12;
            _matrix._13 = mat_temp._13;
            _matrix._21 = mat_temp._21;
            _matrix._22 = mat_temp._22;
            _matrix._23 = mat_temp._23;
            _matrix._31 = mat_temp._31;
            _matrix._32 = mat_temp._32;
            _matrix._33 = mat_temp._33;
        }

        public Matrix2D()
        {
            Identity();
        }

        public void Identity()
        {
            //_matrix.Reset();
            _matrix._11 = 1; _matrix._12 = 0; _matrix._13 = 0;

            _matrix._21 = 0; _matrix._22 = 1; _matrix._23 = 0;

            _matrix._31 = 0; _matrix._32 = 0; _matrix._33 = 1;
        }

        public void Translate(double x, double y)
        {
            Matrix mat = new Matrix();

            mat._11 = 1; mat._12 = 0; mat._13 = 0;

            mat._21 = 0; mat._22 = 1; mat._23 = 0;

            mat._31 = x; mat._32 = y; mat._33 = 1;

            //and multiply
            MatrixMultiply(mat);
            //_matrix.Translate((float)x, (float)y);
        }

        public void Scale(double xScale, double yScale)
        {
            Matrix mat = new Matrix();

            mat._11 = xScale; mat._12 = 0; mat._13 = 0;

            mat._21 = 0; mat._22 = yScale; mat._23 = 0;

            mat._31 = 0; mat._32 = 0; mat._33 = 1;

            //and multiply
            MatrixMultiply(mat);
            //_matrix.Scale((float)xScale, (float)yScale);

        }

        public void Rotate(double rotation)
        {
            Matrix mat = new Matrix();

            double sin = Math.Sin(rotation);
            double cos = Math.Cos(rotation);

            mat._11 = cos; mat._12 = sin; mat._13 = 0;

            mat._21 = -sin; mat._22 = cos; mat._23 = 0;

            mat._31 = 0; mat._32 = 0; mat._33 = 1;

            //and multiply
            MatrixMultiply(mat);
            //_matrix.Rotate((float)rotation);

        }

        //create a rotation matrix from a fwd and side 2D vector
        public void Rotate(Vector2D fwd, Vector2D side)
        {
            //Vector2D tempForward = new Vector2D(fwd);
            //Vector2D tempSide = new Vector2D(side);

            //tempForward.Normalize();
            //tempSide.Normalize();

            //double angle;
            ////if (Math.Abs(fwd.X) < Constants.Epsilon)
            ////{
            ////    angle = 90;
            ////}
            ////else
            //{
            //    angle = Math.Atan2(fwd.X, fwd.Y);
            //}
            // _matrix.Rotate((float)angle);

            Matrix mat = new Matrix();

            mat._11 = fwd.X; mat._12 = fwd.Y; mat._13 = 0;

            mat._21 = side.X; mat._22 = side.Y; mat._23 = 0;

            mat._31 = 0; mat._32 = 0; mat._33 = 1;

            //and multiply
            MatrixMultiply(mat);

        }

        //applys a transformation matrix to a std::vector of points
        public void TransformVector2Ds(List<Vector2D> vectors)
        {
            for (int pointIndex = 0; pointIndex < vectors.Count; pointIndex++)
            {
                Vector2D point = vectors[pointIndex];

                double tempX = (_matrix._11 * point.X) + (_matrix._21 * point.Y) + (_matrix._31);
                double tempY = (_matrix._12 * point.X) + (_matrix._22 * point.Y) + (_matrix._32);

                point.X = tempX;
                point.Y = tempY;
            }

            //PointF[] pointArray = new PointF[vectors.Count];
            //for (int vectorIndex = 0; vectorIndex < vectors.Count; vectorIndex++)
            //{
            //    pointArray[vectorIndex] = vectors[vectorIndex].ToPoint();
            //}

            //_matrix.TransformPoints(pointArray);

            //// now copy back over
            //for (int vectorIndex = 0; vectorIndex < vectors.Count; vectorIndex++)
            //{
            //    vectors[vectorIndex].X = pointArray[vectorIndex].X;
            //    vectors[vectorIndex].Y = pointArray[vectorIndex].Y;
            //}
        }

        public void TransformVector2Ds(Vector2D point)
        {
            double tempX = (_matrix._11 * point.X) + (_matrix._21 * point.Y) + (_matrix._31);
            double tempY = (_matrix._12 * point.X) + (_matrix._22 * point.Y) + (_matrix._32);

            point.X = tempX;
            point.Y = tempY;
            //PointF[] pointF = new PointF[] { point.ToPoint() };
            //_matrix.TransformPoints(pointF);
            //point.X = pointF[0].X;
            //point.Y = pointF[0].Y;
        }

        //accessors to the matrix elements
        public double _11
        {
            set { _matrix._11 = value; }
        }

        public double _12
        {
            set { _matrix._12 = value; }
        }

        public double _13
        {
            set { _matrix._13 = value; }
        }

        public double _21
        {
            set { _matrix._21 = value; }
        }

        public double _22
        {
            set { _matrix._22 = value; }
        }

        public double _23
        {
            set { _matrix._23 = value; }
        }

        public double _31
        {
            set { _matrix._31 = value; }
        }

        public double _32
        {
            set { _matrix._32 = value; }
        }

        public double _33
        {
            set { _matrix._33 = value; }
        }
    }
}
