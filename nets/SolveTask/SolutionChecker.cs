using System;
using System.Collections.Generic;
using System.Configuration;
using DataClassLibrary;
using SolveTask.Server;
using SolveTask.ServerCodeGenerators;
using SolveTask.Logging;
using System.IO;

namespace SolveTask
{
    public static class SolutionChecker
    {
        private static PlacementsStorage positions;

        private static readonly ConsoleLogger logger = new ConsoleLogger();
        private static readonly ServerCluster prologCluster = new ServerCluster(File.ReadAllLines(ConfigurationManager.AppSettings.Get("serverAddressesFile")));

        public static List<List<int>> FindAnAnswer(List<Figure> data, int w, int h, List<double> scaleCoefs)
        {
            positions = new PlacementsStorage();

            ReplaceFiguresWithIndexes(data, out var indexes);
            FillLists(w, h, out var widthScaled, out var heightScaled, scaleCoefs);

            return GetWorkingArrangementPreDefFigs(indexes, new List<double>(scaleCoefs), widthScaled, heightScaled);
        }

        #region Вспомогательные функции 
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

        /// <summary>
        /// Заполнение массива индексов для поиска результата
        /// </summary>
        private static void ReplaceFiguresWithIndexes(List<Figure> data, out List<int> indexes)
		{
            indexes = new List<int>();

            foreach (Figure f in data)
            {
                for (int i = 0; i < f.amount; i++)
                    indexes.Add(f.id);
            }
        }

        /// <summary>
        /// Заполнение массивов размеров листов
        /// </summary>
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
        #endregion

        /// <summary>
        /// Расположение фигур на одном листе
        /// </summary>
        private static ResultData FitCurrentListArrangement(List<int> figInd, List<double> scaleCoefs, List<int> w, List<int> h)
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

                if (FitCurrentListArrangement(newCurLst, scaleCoefs, w, h) != null)
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
