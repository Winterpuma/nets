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
        Dictionary<Task, (List<int>, List<int>, List<List<int>>, bool)> tasksToArrangement;
        
        List<double> scaleCoefs;
        List<int> w;
        List<int> h;

        /// <summary>
        /// Расположение фигур на одном листе
        /// </summary>
        private ResultData FitCurrentListArrangement(List<int> figInd, List<int> backedData = null, List<List<int>> backedResult = null)
        {
            if (!Program.IsMainThread)
                logger.LogError("not in main thread");

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

            if (IsArrangementInProgress(figInd))
            {
                logger.Log("Данное размещение уже проверяется.");
                if (Program.IsMainThread)
				{
                    logger.LogError("Т.к. в главной ветке, ожидаем результата.");
                    var task = GetKeyByArrangement(figInd);
                    var tmp = tasksToArrangement[task];
                    tmp.Item4 = false;
                    tasksToArrangement[task] = tmp;
                    SpinWait.SpinUntil(() => task.IsCompleted);
                    logger.LogError("Задача закончилась.");
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
                else
                {
                    logger.LogError("Прекращаем выполнение в этой ветке.");
                    Thread.CurrentThread.Abort();
				}
                return null;
            }

            var findingTask = prologCluster.StartAnyResultTask(w, h, scaleCoefs, figInd);
            var finishedAfterFirstDelay = SpinWait.SpinUntil(() => findingTask.IsCompleted, timeout); // ожидание таймаута

            if (!finishedAfterFirstDelay && Program.IsMainThread)
            {
                var nextFigInd = MyCopy(figInd);
                nextFigInd[nextFigInd.Count - 1]++; //след фигура //TODO:а если нет
                logger.Log("Добавление другой задачи");
                logger.Log(nextFigInd);
                var nextTask = prologCluster.StartAnyResultTask(w, h, scaleCoefs, nextFigInd);
                tasksToArrangement.Add(nextTask, (nextFigInd, RemoveAllFirst(backedData), backedResult, true)); //other back?????
                nextTask.ContinueWith(delegate { HandleResult(nextTask); });
			}

            SpinWait.SpinUntil(() => findingTask.IsCompleted);
            if (findingTask.IsCanceled || findingTask.IsFaulted)
			{
                positions.AddBadPos(figInd);
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
            foreach (KeyValuePair<Task, (List<int>, List<int>, List<List<int>>, bool)> kvp in tasksToArrangement)
			{
                if (PlacementsStorage.IsTwoArrangementsEqual(arrangement, kvp.Value.Item1))
                    return true;
			}

            return false;
        }

        private Task GetKeyByArrangement(List<int> arrangement)
        {
            foreach (KeyValuePair<Task, (List<int>, List<int>, List<List<int>>, bool)> kvp in tasksToArrangement)
            {
                if (PlacementsStorage.IsTwoArrangementsEqual(arrangement, kvp.Value.Item1))
                    return kvp.Key;
            }

            return null;
        }
        private List<int> RemoveAllFirst(List<int> list)
		{
            int first = list[0];

            return list.FindAll((int el) => el != first);
		}

        private ResultData HandleResult(Task<HttpResponseMessage> task)
        {
            // TODO: delete from in progress
            var backed = tasksToArrangement[task];
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

            if (backed.Item4)
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

            tasksToArrangement = new Dictionary<Task, (List<int>, List<int>, List<List<int>>, bool)>();

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
            if (!Program.IsMainThread)
            {
                return null;
            }
            
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
