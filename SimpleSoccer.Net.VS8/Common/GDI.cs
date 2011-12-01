using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SimpleSoccer.Net
{
    internal static class GDI
    {
        private static Font _textFont;

        private static Pen _currentPen = Pens.Transparent;
        private static Brush _currentBrush = new SolidBrush(Color.Transparent);
        private static Color _textColor = Color.Transparent;

        public static Pen CurrentPen
        {
            get { return _currentPen; }
            set { _currentPen = value; }
        }

        public static Brush CurrentBrush
        {
            get { return _currentBrush; }
            set { _currentBrush = value; }
        }

        public static Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        public static Font TextFont
        {
            get { return _textFont; }
        }

        static GDI()
        {
            _textFont = new Font("Microsoft Sans Serif", 10.0f, FontStyle.Bold, GraphicsUnit.Point);
        }

        public static void DrawText(Graphics g, Vector2D location, string format, params object[] formatParameters)
        {
            string text = string.Format(format, formatParameters);
            DrawText(g, location, text);
        }

        public static void DrawText(Graphics g, PointF location, string format, params object[] formatParameters)
        {
            string text = string.Format(format, formatParameters);
            DrawText(g, location, text);
        }

        public static void DrawText(Graphics g, PointF location, string text)
        {
            g.DrawString(text, _textFont, new SolidBrush(_textColor), location);
        }

        public static void DrawText(Graphics g, Vector2D location, string text)
        {
            g.DrawString(text, _textFont, new SolidBrush(_textColor), new Point((int)location.X, (int)location.Y));
        }

        public static void DrawCircle(Graphics g, Vector2D location, double radius)
        {
            float x = (float)location.X - (float)radius;
            float y = (float)location.Y - (float)radius;

            g.FillEllipse(_currentBrush, x, y, (float)radius * 2.0f, (float)radius * 2.0f);
            g.DrawEllipse(_currentPen, x, y, (float)radius * 2.0f, (float)radius * 2.0f);
        }

        public static void DrawPolygon(Graphics g, List<Vector2D> vectors)
        {
            PointF[] points = new PointF[vectors.Count];
            for (int i = 0; i < vectors.Count; i++)
            {
                points[i] = new PointF((float)vectors[i].X, (float)vectors[i].Y);
            }

            ////render the player's body
            g.DrawPolygon(_currentPen, points);

            ////MoveToEx(m_hdc, (int)points[0].x, (int)points[0].y, NULL);

            //for (int p = 0; p < vectors.Count -1; ++p)
            //{
            //    //LineTo(m_hdc, (int)points[p].x, (int)points[p].y);
            //    g.DrawLine(_currentPen, vectors[p].ToPoint(), vectors[p + 1].ToPoint());
            //}

            //// close polygon
            //g.DrawLine(_currentPen, vectors[vectors.Count - 1].ToPoint(), vectors[0].ToPoint());

        }

        public static void DrawVector(Graphics g, Vector2D position, Vector2D lookAt)
        {
            Matrix2D mat = new Matrix2D();

            mat.Translate(position.X, position.Y);

            Vector2D lookAtCopy = new Vector2D(position.X + lookAt.X, position.Y + lookAt.Y);
            //lookAtCopy *= .5;

            //mat.TransformVector2Ds(lookAtCopy);

            g.DrawLine(_currentPen, new Point((int)position.X, (int)position.Y), new Point((int)lookAtCopy.X, (int)lookAtCopy.Y));
        }
    }
}
