using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Andrew
{
    public partial class Form1 : Form
    {
        private Pen pen;
        private Pen redPen;
        private Graphics g;
        private List<Point> points;
        private List<Point> convexPolygon;

        public Form1()
        {
            InitializeComponent();

            pen = new Pen(Color.Black, 1);
            pen.StartCap = pen.EndCap = LineCap.Round;
            redPen = new Pen(Color.Red, 3);
            Image bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            g = Graphics.FromImage(bmp);
            pictureBox.Image = bmp;

            points = new List<Point>();
            convexPolygon = new List<Point>();
        }

        private void Andrew()
        {
            if (points.Count < 2) return;
            var sortedPoints = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            List<Point> upperHull = new List<Point>();
            List<Point> lowerHull = new List<Point>();
            Point leftmostPoint = sortedPoints.First();
            Point rightmostPoint = sortedPoints.Last();
            upperHull.Add(leftmostPoint);
            lowerHull.Add(leftmostPoint);

            int totalPoints = sortedPoints.Count;

            for (int i = 1; i < totalPoints; ++i)
            {
                Point currentPoint = sortedPoints[i];
                if (i == totalPoints - 1 || CrossProduct(leftmostPoint, currentPoint, rightmostPoint) < 0)  
                {
                    while (upperHull.Count >= 2 && CrossProduct(upperHull[upperHull.Count - 2], upperHull[upperHull.Count - 1], currentPoint) >= 0) // Тут проверяем, что точка upperHull[upperHull.Count - 1] не справа
                    {
                        upperHull.RemoveAt(upperHull.Count - 1);
                    }
                    upperHull.Add(currentPoint);
                }
                if (i == totalPoints - 1 || CrossProduct(leftmostPoint, currentPoint, rightmostPoint) > 0)
                {
                    while (lowerHull.Count >= 2 && CrossProduct(lowerHull[lowerHull.Count - 2], lowerHull[lowerHull.Count - 1], currentPoint) <= 0) // Тут проверяем, что точка upperHull[upperHull.Count - 1] не слева
                    {
                        lowerHull.RemoveAt(lowerHull.Count - 1);
                    }
                    lowerHull.Add(currentPoint);
                }
            }
            upperHull.RemoveAt(upperHull.Count - 1);
            lowerHull.Skip(0);
            convexPolygon.AddRange(upperHull);
            convexPolygon.AddRange(lowerHull.AsEnumerable().Reverse());
        }

        private int CrossProduct(Point p1, Point p2, Point p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        }

        private void DrawDot(object sender, MouseEventArgs e)
        {
            g.DrawEllipse(redPen, new Rectangle(e.X - 2, e.Y - 2, 4, 4));
            points.Add(new Point(e.X, e.Y));
            pictureBox.Invalidate();
            UpdateBuildButton();
        }

        private void BuildClick(object sender, EventArgs e)
        {
            if (points.Count < 2) return; 
            g.Clear(Color.White);
            foreach (var point in points)
            {
                g.DrawEllipse(redPen, new Rectangle(point.X - 2, point.Y - 2, 4, 4));
            }

            Andrew();

            for (int i = 0; i < convexPolygon.Count - 1; ++i)
                g.DrawLine(pen, convexPolygon[i], convexPolygon[i + 1]);
            g.DrawLine(pen, convexPolygon[0], convexPolygon[convexPolygon.Count - 1]);
            pictureBox.Invalidate();
            convexPolygon.Clear();
        }

        private void ClearClick(object sender, EventArgs e)
        {
            if (points.Count > 0)
                points.Clear();
            if (convexPolygon.Count > 0)
                convexPolygon.Clear();
            g.Clear(Color.White);
            pictureBox.Invalidate();
            UpdateBuildButton(); 
        }

        private void UpdateBuildButton()
        {
            button1.Enabled = points.Count > 2;
        }
    }
}
