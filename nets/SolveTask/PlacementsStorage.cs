using System.Collections.Generic;

namespace SolveTask
{
	class PlacementsStorage
    {
        private List<List<int>> badPositions;
        private List<List<int>> goodPositions;

        public PlacementsStorage()
        {
            badPositions = new List<List<int>>();
            goodPositions = new List<List<int>>(); // а можно запоминать и рез (словарик там)
        }

        public void AddBadPos(List<int> badPositioning)
        {
            badPositions.Add(badPositioning);
        }

        public void AddGoodPos(List<int> goodPositioning)
        {
            goodPositions.Add(goodPositioning);
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
            foreach (List<int> curBP in goodPositions)
            {
                if (Equals(pos, curBP))
                    return true;
            }
            return false;
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
