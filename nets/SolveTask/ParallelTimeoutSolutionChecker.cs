using System;
using System.Collections.Generic;
using System.Configuration;
using DataClassLibrary;
using SolveTask.Server;
using SolveTask.ServerCodeGenerators;
using SolveTask.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace SolveTask
{
    /// <summary>
    /// Если за определенное время не было найдено расположение начинаем подсчитывать следующие
    /// </summary>
    public class ParallelTimeoutSolutionChecker : SolutionCheckerBase
    {
        TimeSpan timeout = TimeSpan.FromSeconds(20);
        Dictionary<int, (List<int>, List<int>, List<List<int>>)> tasksToArrangement;
        
        List<double> scaleCoefs;
        List<int> w;
        List<int> h;

        /// <summary>
        /// Расположение фигур на одном листе
        /// </summary>
        private ResultData FitCurrentListArrangement(List<int> figInd, List<int> backedData = null, List<List<int>> backedResult = null)
        {
            if (IsArrangementInProgress(figInd))
			{
                logger.Log("Данное размещение уже проверяется.");
                return null;
            }

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

            var findingTask = prologCluster.StartAnyResultTask(w, h, scaleCoefs, figInd);
            var finished = SpinWait.SpinUntil(() => findingTask.IsCompleted, timeout); // ожидание таймаута

            if (!finished)
            {
                logger.Log("Переключение на другую задачу");
                tasksToArrangement.Add(findingTask.Id, (figInd, backedData, backedResult));
                findingTask.ContinueWith(delegate { HandleResult(findingTask); });
                return null;
			}

            string responce = findingTask.Result.Content.ReadAsStringAsync().Result;
            var res = PrologServer.HandleServerResponce(responce);

            if (res == null)
            {
                positions.AddBadPos(figInd);
                return null;
            }

            positions.AddGoodPos(figInd, res);
            logger.Log(res);

            return res;
        }

        private bool IsArrangementInProgress(List<int> arrangement)
		{
            foreach (KeyValuePair<int, (List<int>, List<int>, List<List<int>>)> kvp in tasksToArrangement)
			{
                if (PlacementsStorage.IsTwoArrangementsEqual(arrangement, kvp.Value.Item1))
                    return true;
			}

            return false;
        }

        private ResultData HandleResult(Task<HttpResponseMessage> task)
        {
            var backed = tasksToArrangement[task.Id];
            var figInd = backed.Item1;

            if (task.IsCanceled || task.IsFaulted)
            {
                positions.AddBadPos(figInd);
                return null;
            }

            string responce = task.Result.Content.ReadAsStringAsync().Result;
            var res = PrologServer.HandleServerResponce(responce);

            if (res == null)
            {
                positions.AddBadPos(figInd);
                return null;
            }

            positions.AddGoodPos(figInd, res);

            GetWorkingArrangement(backed.Item2, backed.Item3);

            return res;
        }

        /// <summary>
        /// Рекурсивное расположение всех фигур
        /// </summary>
        public override List<List<int>> GetWorkingArrangement(List<int> data, List<double> scaleCoefs, List<int> w, List<int> h,
            List<List<int>> result = null)
		{
            this.scaleCoefs = scaleCoefs;
            this.w = w;
            this.h = h;

            tasksToArrangement = new Dictionary<int, (List<int>, List<int>, List<List<int>>)>();

            return GetWorkingArrangement(data, result);
		}

        public List<List<int>> GetWorkingArrangement(List<int> data, List<List<int>> result = null)
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

                if (FitCurrentListArrangement(newCurLst, MyCopy(data), MyCopy(result)) != null)
                {
                    result[i].Add(currentFig);
                    var curResult = GetWorkingArrangement(nextData, result);
                    if (curResult != null)
                        return curResult;
                }
            }

			// Если не получилось добавить к существующим, кладем в новый
			var tmp = new List<int> { currentFig };
			var newResult = new List<List<int>>(result) { tmp };
			var curRes = GetWorkingArrangement(nextData, newResult);

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

                var res = FitCurrentListArrangement(figInd);
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
