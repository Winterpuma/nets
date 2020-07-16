using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PictureWork
{
    public class DeltaRepresentation
    {
        Bitmap bmp = null;
        int xCenter, yCenter;
        public double angle = 0;

        public List<Point> deltas = new List<Point>();


        public DeltaRepresentation() { }

        public DeltaRepresentation(Bitmap bmp, Color figColor)
        {
            this.bmp = bmp;

            xCenter = bmp.Width / 2;
            yCenter = bmp.Height / 2;

            for (int xCur = 0; xCur < bmp.Width; xCur++)
            {
                for (int yCur = 0; yCur < bmp.Height; yCur++)
                {
                    Color curColor = bmp.GetPixel(xCur, yCur);
                    if (curColor.R == figColor.R && curColor.G == figColor.G && curColor.B == figColor.B)
                    {
                        int xDelta = xCenter - xCur;
                        int yDelta = yCenter - yCur;

                        deltas.Add(new Point(xDelta, yDelta));
                    }
                }
            }
        }


        /// <summary>
        /// Возвращает повернутую DeltaRepresentation на заданный угол относительно заданной точки
        /// </summary>
        public DeltaRepresentation GetTurnedDelta(double angle, int centerX, int centerY)
        {
            DeltaRepresentation res = new DeltaRepresentation();
            res.angle = this.angle + angle;

            List<Point> newDeltas = new List<Point>();

            foreach (Point p_old in deltas)
            {
                double newX = centerX + (p_old.X - centerX) * Math.Cos(angle) + (p_old.Y - centerY) * Math.Sin(angle);
                double newY = centerY - (p_old.X - centerX) * Math.Sin(angle) + (p_old.Y - centerY) * Math.Cos(angle);
                
                newDeltas.Add(new Point((int)newX, (int)newY));
                // также добавляем точки вокруг, чтобы устранить дырки
                newDeltas.Add(new Point((int)newX+1, (int)newY));
                newDeltas.Add(new Point((int)newX-1, (int)newY));
            }

            // Убирает дубликаты
            res.deltas = newDeltas.Distinct().ToList();

            return res;
        }

        public static SortedDictionary<int, List<int>> TransformDeltaToDict(List<Point> deltas)
        {
            SortedDictionary<int, List<int>> res = new SortedDictionary<int, List<int>>();

            foreach (Point curDelta in deltas)
            {
                if (res.ContainsKey(curDelta.Y))
                {
                    res[curDelta.Y].Add(curDelta.X);
                }
                else
                {
                    var tmp = new List<int>();
                    tmp.Add(curDelta.X);
                    res.Add(curDelta.Y, tmp);
                }
            }

            return res;
        }

        public SortedDictionary<int, List<int>> TransformDeltaToDict()
        {
            return TransformDeltaToDict(deltas);
        }


        /// <summary>
        /// Получает 4 точки фигуры, лежащие на прямоугольной оболочке
        /// </summary>
        /// <returns></returns>
        public SortedDictionary<int, List<int>> GetOuterDots()
        {
            Point minX = new Point(int.MaxValue, int.MaxValue);
            Point minY = new Point(int.MaxValue, int.MaxValue);
            Point maxY = new Point(int.MinValue, int.MinValue);
            Point maxX = new Point(int.MinValue, int.MinValue);

            foreach (Point p in deltas)
            {
                if (minX.X > p.X)
                    minX = p;
                if (minY.Y > p.Y)
                    minY = p;
                if (maxX.X < p.X)
                    maxX = p;
                if (maxY.Y < p.Y)
                    maxY = p;
            }

            List<Point> outerDots = new List<Point>{ minX, minY, maxX, maxY };
            return TransformDeltaToDict(outerDots);
        }
    }
}
