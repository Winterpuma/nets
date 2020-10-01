using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataClassLibrary
{
    public class SegmentRepresentation
    {
        /// <summary>
        /// Ключ - кордината по у, значение - список отрезков на этой координате
        /// </summary>
        public SortedDictionary<int, List<Segment>> segments = new SortedDictionary<int, List<Segment>>();

        public SegmentRepresentation(SortedDictionary<int, List<int>> yGroups)
        {
            foreach(KeyValuePair<int, List<int>> curGroup in yGroups)
                segments.Add(curGroup.Key, GetSegmentsOutOfLine(curGroup.Value));
        }

        /// <summary>
        /// Получает диапазон высоты
        /// </summary>
        /// <returns>Строка вида (-2,5)</returns>
        public string GetMinMaxYLine()
        {
            return "(" + segments.Keys.Min() + "," + segments.Keys.Max() + ")";
        }


        /// <summary>
        /// Разбивает полоску на сегменты
        /// </summary>
        /// <param name="line">Задает закрашенные элементы на прямой</param>
        /// <returns>Список сегментов</returns>
        private List<Segment> GetSegmentsOutOfLine(List<int> line)
        {
            List<Segment> res = new List<Segment>();
            line.Sort();

            int start = line.First();
            int prev = start - 1;

            foreach (int point in line)
            {
                if (point != prev + 1) // если сегмент прервался
                {
                    res.Add(new Segment { start = start, end = prev });
                    start = point;
                }
                prev = point;
            }
            res.Add(new Segment { start = start, end = line.Last() });
            return res;
        }

    }

    public class Segment
    {
        public int start;
        public int end;
        
        override public string ToString()
        {
            return "(" + start + "," + end + ")";
        }
    }
}
