using System.Collections.Generic;
using DataClassLibrary;

namespace SolveTask
{
    public class SolutionChecker : SolutionCheckerBase
    {
        /// <summary>
        /// Расположение фигур на одном листе
        /// </summary>
        private ResultData FitCurrentListArrangement(List<int> figInd, List<double> scaleCoefs, List<int> w, List<int> h)
        {
            if (positions.IsPosBad(figInd))
            {
                logger.Log("Позиция известна как неподходящая.");
                return null;
            }

            if (positions.IsPosGood(figInd))
            {
                logger.Log("Позиция известна как подходящая.");
                return positions.GetGoodPosition(figInd);
            }

            ResultData res = prologCluster.GetAnyResult(w, h, scaleCoefs, figInd);
            if (res == null)
            {
                positions.AddBadPos(figInd);
                return null;
            }

            positions.AddGoodPos(figInd, res);
            logger.Log(res);

            return res;
        }

        /// <summary>
        /// Рекурсивное расположение всех фигур
        /// </summary>
        public override List<List<int>> GetWorkingArrangement(List<int> data, List<double> scaleCoefs, List<int> w, List<int> h,
            List<List<int>> result = null)
        {
            if (result == null)
            {
				result = new List<List<int>> { new List<int>() };

				result[0].Add(data[0]); // первую фигуру всегда в новый лист
                data.RemoveAt(0);
            }
            if (data.Count == 0)
            {
                logger.Log(result);
                return result;
            }

            int currentFig = data[0];
            var nextData = new List<int>(data);
            nextData.RemoveAt(0);


            // Пытаемся последовательно добавлять в уже существующие листы
            for (int i = 0; i < result.Count; i++)
            {
				var newCurLst = new List<int>(result[i]) { currentFig };

                logger.Log(newCurLst);

                if (FitCurrentListArrangement(newCurLst, scaleCoefs, w, h) != null)
                {
                    result[i].Add(currentFig);
                    var curResult = GetWorkingArrangement(nextData, scaleCoefs, w, h, result);
                    if (curResult != null)
                        return curResult;
                }
            }

			// Если не получилось добавить к существующим, кладем в новый
			var tmp = new List<int> { currentFig };
			var newResult = new List<List<int>>(result) { tmp };
			var curRes = GetWorkingArrangement(nextData, scaleCoefs, w, h, newResult);

			return curRes ?? null;
		}

		/// <summary>
		/// Известно разделение фигур по листам, ищется расположение
		/// </summary>
		public List<ResultData> PlacePreDefinedArrangement(List<List<int>> arrangement, int wLast, int hLast, List<double> scaleCoefs)
        {
            List<ResultData> results = new List<ResultData>();
            foreach (List<int> figInd in arrangement)
            {
                FillLists(wLast, hLast, out var w, out var h, scaleCoefs);

                var res = FitCurrentListArrangement(figInd, scaleCoefs, w, h);
                if (res == null)
                {
                    logger.LogError("Ошибка: невозможно расположиться фигуры при заданном распределении");
                    return null;
                }
                else
                {
                    results.Add(res);
                }
            }
            return results;
        }
    }
}
