using System;
using System.Collections.Generic;
using System.Configuration;
using DataClassLibrary;
using SolveTask.Server;
using SolveTask.ServerCodeGenerators;
using SolveTask.Logging;

namespace SolveTask
{
    public static class SolutionChecker
    {
        private static readonly ConsoleLogger logger = new ConsoleLogger();

        private static List<List<int>> badPositions;
        private static List<List<int>> goodPositions;

        private static readonly ServerCluster prologCluster = new ServerCluster(new List<string>() { ConfigurationManager.AppSettings.Get("serverAdress") });

        private static void InitPos()
        {
            badPositions = new List<List<int>>();
            goodPositions = new List<List<int>>(); // а можно запоминать и рез (словарик там)
        }

        private static void AddBadPos(List<int> badPositioning)
        {
            badPositions.Add(badPositioning);
        }

        private static void AddGoodPos(List<int> goodPositioning)
        {
            //goodPositions.Add(goodPositioning);
        }

        private static bool IsPosBad(List<int> pos)
        {
            foreach (List<int> curBP in badPositions)
            {
                if (Equals(pos, curBP))
                    return true;
            }
            return false;
        }

        private static bool IsPosGood(List<int> pos)
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

        #region Проверки 
        /// <summary>
        /// Считает сумму дельт
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static int CountDeltas(List<Figure> data)
        {
            int sum = 0;
            foreach(Figure f in data)
            {
                sum += f[0].Count;
            }
            return sum;
        }

        /// <summary>
        /// Загрузка фигур в файл пролога и выгрузка на сервер
        /// </summary>
        public static void LoadFigures(List<Figure> data, List<double> scaleCoefs)
        {
            string tmpFilename = "tmpFigInfo.pl";
            FigureFileOperations.CreateNewFigFile(tmpFilename);//pathProlog + "figInfo.pl");
            FigureFileOperations.AddManyFigs(data, scaleCoefs);
            prologCluster.UploadFileToCluster(tmpFilename, "figInfo.pl");
        }

        public static List<List<int>> FindAnAnswer(List<Figure> data, int w, int h, List<double> scaleCoefs)
        {
            // Заполнение массива индексов для поиска результата
            List<int> indexes = new List<int>();
            foreach (Figure f in data)
            {
                for (int i = 0; i < f.amount; i++)
                    indexes.Add(f.id);
            }

			// Заполнение массивов размеров листов
			FillLists(w, h, out var widthScaled, out var heightScaled, scaleCoefs);

			InitPos();
            return GetWorkingArrangementPreDefFigs(indexes, new List<double>(scaleCoefs), widthScaled, heightScaled);
        }

        private static void FillLists(int wLast, int hLast, out List<int> w, out List<int> h, List<double> scaleCoefs)
        {
            w = new List<int>();
            h = new List<int>();
            foreach (double d in scaleCoefs)
            {
                w.Add((int)(wLast * d));
                h.Add((int)(hLast * d));
            }
        }

        private static ResultData DoesCurrentListFitPreDefFigs2(List<int> figInd, List<double> scaleCoefs, List<int> w, List<int> h)
        {
            if (IsPosBad(figInd))
                return null;

            if (IsPosGood(figInd))
                return new ResultData(); 

            ResultData res = prologCluster.GetAnyResult(w, h, scaleCoefs, figInd);
            if (res == null)
            {
                AddBadPos(figInd);
                return null;
            }

            AddGoodPos(figInd);
            logger.Log(res);
            //res.SetLstInfo(w.FindLast(), newH, scaleCoefs[0]);

            return res;
        }

        private static bool DoesCurrentListFitPreDefFigs(List<int> figInd, List<double> scaleCoefs, List<int> w, List<int> h)
        {
            var res = DoesCurrentListFitPreDefFigs2(figInd, scaleCoefs, w, h);

            return res != null;
        }
        #endregion

        public static List<List<int>> GetWorkingArrangementPreDefFigs(List<int> data, List<double> scaleCoefs, List<int> w, List<int> h,
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

                if (DoesCurrentListFitPreDefFigs(newCurLst, scaleCoefs, w, h))
                {
                    result[i].Add(currentFig);
                    var curResult = GetWorkingArrangementPreDefFigs(nextData, scaleCoefs, w, h, result);
                    if (curResult != null)
                        return curResult;
                }
            }

			// Если не получилось добавить к существующим, кладем в новый
			var tmp = new List<int> { currentFig };
			var newResult = new List<List<int>>(result) { tmp };
			var curRes = GetWorkingArrangementPreDefFigs(nextData, scaleCoefs, w, h, newResult);

			return curRes ?? null;
		}

		/// <summary>
		/// Известно разделение фигур по листам, ищется расположение
		/// </summary>
		public static List<ResultData> PlacePreDefinedArrangement(List<List<int>> arrangement, int wLast, int hLast, List<double> scaleCoefs)
        {
            List<ResultData> results = new List<ResultData>();
            foreach (List<int> figInd in arrangement)
            {
                FillLists(wLast, hLast, out var w, out var h, scaleCoefs);

                var res = DoesCurrentListFitPreDefFigs2(figInd, scaleCoefs, w, h);
                if (res == null)
                {
                    logger.LogError("Error: can't fit figures with given arrangement");
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
