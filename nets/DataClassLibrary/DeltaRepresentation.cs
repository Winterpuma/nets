using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DataClassLibrary
{
    public class DeltaRepresentation
    {
        Bitmap bmp = null;
        public int xCenter, yCenter;
        public double angle = 0;

        public List<Point> deltas = new List<Point>();
        SortedDictionary<int, List<int>> dictRepresentation = null;
        SortedDictionary<int, List<int>> outline = null;
        SortedDictionary<int, List<int>> specialDots = null;



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
                    if (IsColorsEqual(curColor, figColor))
                    {
                        int xDelta = xCenter - xCur;
                        int yDelta = yCenter - yCur;

                        deltas.Add(new Point(xDelta, yDelta));
                    }
                }
            }
        }

        public DeltaRepresentation(Bitmap bmp, Color figColor, int scaleBorders)
        {
            this.bmp = bmp;

            xCenter = bmp.Width / 2;
            yCenter = bmp.Height / 2;

            for (int xCur = 0; xCur < bmp.Width; xCur++)
            {
                for (int yCur = 0; yCur < bmp.Height; yCur++)
                {
                    Color curColor = bmp.GetPixel(xCur, yCur);
                    if (IsColorsEqual(curColor, figColor))
                    {
                        int xDelta = xCenter - xCur;
                        int yDelta = yCenter - yCur;


                        for (int i = -scaleBorders; i <= scaleBorders; i++)
                            for (int j = -scaleBorders; j <= scaleBorders; j++)
                                deltas.Add(new Point(xDelta + i, yDelta + j));
                    }
                }
            }

            deltas = deltas.Distinct().ToList();
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


        public SortedDictionary<int, List<int>> GetDictRepresentation()
        {
            if (dictRepresentation == null)
                dictRepresentation = TransformDeltaToDict(deltas);
            return dictRepresentation;
        }


        private static SortedDictionary<int, List<int>> TransformDeltaToDict(List<Point> deltas)
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


        /// <summary>
        /// Получает 4 точки фигуры, лежащие на прямоугольной оболочке
        /// </summary>
        /// <returns></returns>
        public SortedDictionary<int, List<int>> GetOuterDots()
        {
            if (specialDots != null)
                return specialDots;

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
            specialDots = TransformDeltaToDict(outerDots);
            return specialDots;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Возвращает контур дельты</returns>
        public SortedDictionary<int, List<int>> GetOutline()
        {
            if (outline != null)
                return outline;

            outline = new SortedDictionary<int, List<int>>();

            foreach (KeyValuePair<int, List<int>> yGroup in GetDictRepresentation())
            {
                var borderDots = new List<int>();

                int prevX = yGroup.Value[0];
                borderDots.Add(prevX);
                prevX--; // чтобы в цикле не добавлять повторно

                foreach (int curX in yGroup.Value)
                {
                    if (prevX + 1 != curX)
                    {
                        borderDots.Add(prevX);
                        borderDots.Add(curX);
                    }
                    prevX = curX;
                }
                borderDots.Add(prevX);

                outline.Add(yGroup.Key, borderDots);
            }
            return outline;
        }


        /// <summary>
        /// Проверяет одинаковые ли цвета
        /// </summary>
        private static bool IsColorsEqual(Color a, Color b)
        {
            return (a.R == b.R && a.G == b.G && a.B == b.B);
        }
    }
}
