using DataClassLibrary;
using System.Collections.Generic;

namespace SolveTask
{
	public class PlacementsStorage
    {
        private List<List<int>> badPositions;
        private List<(List<int>, ResultData)> goodPositions;

        public PlacementsStorage()
        {
            badPositions = new List<List<int>>();
            goodPositions = new List<(List<int>, ResultData)>();
        }

        public void AddBadPos(List<int> badPositioning)
        {
            badPositions.Add(badPositioning);
        }

        public void AddGoodPos(List<int> goodPositioning, ResultData result)
        {
            goodPositions.Add((goodPositioning, result));
        }

        public bool IsPosBad(List<int> pos)
        {
            foreach (List<int> curBP in badPositions)
            {
                if (Equals(pos, curBP))
                    return true;
            }
            return false;
        }

        public bool IsPosGood(List<int> pos)
        {
            foreach ((List<int> curBP, _) in goodPositions)
            {
                if (Equals(pos, curBP))
                    return true;
            }
            return false;
        }

        public ResultData GetGoodPosition(List<int> pos)
        {
            foreach ((List<int> curBP, ResultData res) in goodPositions)
            {
                if (Equals(pos, curBP))
                    return res;
            }
            return null;
        }

        public static bool IsTwoArrangementsEqual(List<int> a, List<int> b)
		{
            return Equals(a, b);
		}

        static bool Equals<T>(List<T> a, List<T> b)
        {
            if (a == null) return b == null;
            if (b == null || a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (!Equals(a[i], b[i]))
                    return false;
            }
            return true;
        }
    }
}
