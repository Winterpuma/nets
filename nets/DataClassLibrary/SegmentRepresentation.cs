using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataClassLibrary
{
    public class SegmentRepresentation
    {
        List<List<Segment>> segments = new List<List<Segment>>();

        public SegmentRepresentation(SortedDictionary<int, List<int>> yGroups)
        {
            foreach(KeyValuePair<int, List<int>> curGroup in yGroups)
                segments.Add(GetSegmentsOutOfLine(curGroup.Value));
        }


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
    }
}
