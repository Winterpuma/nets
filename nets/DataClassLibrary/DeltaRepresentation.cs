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


        /// <summary>
        /// Загрузка пикселей заданного цвета из изображения
        /// </summary>
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
                        int xDelta = xCur;
                        int yDelta = yCur;

                        deltas.Add(new Point(xDelta, yDelta));
                    }
                }
            }
            CenterDeltaRepresentation();
        }


        /// <summary>
        /// Загрузка всех пикселей, кроме белого
        /// </summary>
        public DeltaRepresentation(Bitmap bmp)
        {
            Color backColor = Color.White;
            this.bmp = bmp;

            xCenter = bmp.Width / 2;
            yCenter = bmp.Height / 2;

            for (int xCur = 0; xCur < bmp.Width; xCur++)
            {
                for (int yCur = 0; yCur < bmp.Height; yCur++)
                {
                    Color curColor = bmp.GetPixel(xCur, yCur);
                    if (!IsColorsEqual(curColor, backColor))
                    {
                        int xDelta = xCenter - xCur;
                        int yDelta = yCenter - yCur;

                        deltas.Add(new Point(xDelta, yDelta));
                    }
                }
            }
            CenterDeltaRepresentation();
        }


        /// <summary>
        /// Загрузка дельта представления с дополнительными точками по периметру
        /// </summary>
        /// <param name="bmp">Изображение фигуры</param>
        /// <param name="figColor">Цвет фигуры</param>
        /// <param name="scaleBorders">Прирост в пикселях</param>
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
            CenterDeltaRepresentation();
        }


        /// <summary>
        /// Возвращает повернутую DeltaRepresentation на заданный угол относительно заданной точки
        /// </summary>
        public DeltaRepresentation GetTurnedDelta(double angle, int centerX, int centerY)
        {
            if (angle == 0)
                return this;
            DeltaRepresentation res = new DeltaRepresentation();
            res.angle = this.angle + angle;

            List<Point> newDeltas = new List<Point>();

            double cosAngle = Math.Cos(angle);
            double sinAngle = Math.Sin(angle);


            foreach (Point p_old in deltas)
            {
                double newX = centerX + (p_old.X - centerX) * cosAngle + (p_old.Y - centerY) * sinAngle;
                double newY = centerY - (p_old.X - centerX) * sinAngle + (p_old.Y - centerY) * cosAngle;
                
                newDeltas.Add(new Point((int)newX, (int)newY));
                // также добавляем точки вокруг, чтобы устранить дырки
                newDeltas.Add(new Point((int)newX+1, (int)newY));
                newDeltas.Add(new Point((int)newX-1, (int)newY));
            }

            // Убирает дубликаты
            res.deltas = newDeltas.Distinct().ToList();

            return res;
        }


        /// <summary>
        /// Обертка для получения словаря у-групп
        /// </summary>
        /// <returns></returns>
        public SortedDictionary<int, List<int>> GetDictRepresentation()
        {
            if (dictRepresentation == null)
                dictRepresentation = TransformDeltaToDict(deltas);
            return dictRepresentation;
        }


        /// <summary>
        /// Преобразует список дельт в представление отсортированного по у словаря (у-группы)
        /// </summary>
        /// <param name="deltas"></param>
        /// <returns>Словарь: у - ключ, значение - список х на этом у</returns>
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
        /// Возвращает контур (крайние значения по х)
        /// </summary>
        public SortedDictionary<int, List<int>> GetOutline()
        {
            if (outline != null)
                return outline;

            outline = new SortedDictionary<int, List<int>>();

            foreach (KeyValuePair<int, List<int>> yGroup in GetDictRepresentation())
            {
                var borderDots = new List<int>();
                borderDots.Add(yGroup.Value.Min());
                borderDots.Add(yGroup.Value.Max());

                outline.Add(yGroup.Key, borderDots.Distinct().ToList());
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


        /// <summary>
        /// Центрирует данное представление
        /// Нужно когда изначально в файле фигура была не четко по центру
        /// </summary>
        private void CenterDeltaRepresentation()
        {
            if (deltas.Count == 0)
                throw new Exception("Empty figure, maybe different color?"); ;

            int maxX = deltas.Max((Point p) => p.X);
            int minX = deltas.Min((Point p) => p.X);
            int maxY = deltas.Max((Point p) => p.Y);
            int minY = deltas.Min((Point p) => p.Y);

            int dX = Convert.ToInt32(Math.Floor((double)(maxX + minX) / 2));
            int dY = Convert.ToInt32(Math.Floor((double)(maxY + minY) / 2));
            
            for (int i = 0; i < deltas.Count; i++)
            {
                Point cur = deltas[i];
                deltas[i] = new Point(cur.X - dX, cur.Y - dY);
            }
        }


        /// <summary>
        /// Возвращает ширину фигуры
        /// </summary>
        /// <returns></returns>
        public int GetWidth()
        {
            return deltas.Max(p => p.X) - deltas.Min(p => p.X);
        }


        /// <summary>
        /// Возвращает высоту фигуры
        /// </summary>
        public int GetHeight()
        {
            return deltas.Max(p => p.Y) - deltas.Min(p => p.Y);
        }
    }
}
