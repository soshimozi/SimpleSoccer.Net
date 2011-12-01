using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SimpleSoccer.Net
{
    public class Wall2D
    {
        private Vector2D _vectorFrom = new Vector2D();
        private Vector2D _vectorTo = new Vector2D();
        private Vector2D _vectorNormal = new Vector2D();

        internal Vector2D VectorFrom
        {
            get { return _vectorFrom; }
            set { _vectorFrom = value; calculateNormal(); }
        }

        internal Vector2D VectorTo
        {
            get { return _vectorTo; }
            set { _vectorTo = value; calculateNormal(); }
        }

        internal Vector2D VectorNormal
        {
            get { return _vectorNormal; }
            set { _vectorNormal = value; calculateNormal(); }
        }

        private void calculateNormal()
        {
            Vector2D temp = Vector2D.Vec2DNormalize(_vectorTo - _vectorFrom);

            _vectorNormal.X = -temp.Y;
            _vectorNormal.Y = temp.X;
        }

        public Wall2D() { }

        public Wall2D(Vector2D vectorFrom, Vector2D vectorTo)
        {
            _vectorFrom = vectorFrom;
            _vectorTo = vectorTo;

            calculateNormal();
        }

        public Wall2D(Vector2D vectorFrom, Vector2D vectorTo, Vector2D vectorNormal)
        {
            _vectorFrom = vectorFrom;
            _vectorTo = vectorTo;
            _vectorNormal = vectorNormal;
        }

        public virtual void Render(bool renderNormals, Graphics g)
        {
            g.DrawLine(Pens.White, new Point((int)_vectorFrom.X, (int)_vectorFrom.Y), new Point((int)_vectorTo.X, (int)_vectorTo.Y));

            //render the normals if rqd
            if (renderNormals)
            {
              int midX = (int)((_vectorFrom.X+_vectorTo.X)/2);
              int midY = (int)((_vectorFrom.Y+_vectorTo.Y)/2);

              g.DrawLine(Pens.Black, midX, midY, (int)(midX+(_vectorNormal.X * 5)), (int)(midY+(_vectorNormal.Y * 5)));
            }
        }
    }
}
