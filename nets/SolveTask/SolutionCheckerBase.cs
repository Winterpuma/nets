using DataClassLibrary;
using SolveTask.Logging;
using SolveTask.Server;
using SolveTask.ServerCodeGenerators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace SolveTask
{
	public abstract class SolutionCheckerBase
    {
        protected PlacementsStorage positions;

        protected readonly ConsoleLogger logger = new ConsoleLogger();
        protected readonly ServerCluster prologCluster = new ServerCluster(File.ReadAllLines(ConfigurationManager.AppSettings.Get("serverAddressesFile")));

        public List<List<int>> FindAnAnswer(List<Figure> data, int w, int h, List<double> scaleCoefs)
        {
            positions = new PlacementsStorage();

            ReplaceFiguresWithIndexes(data, out var indexes);
            FillLists(w, h, out var widthScaled, out var heightScaled, scaleCoefs);

            return GetWorkingArrangement(indexes, new List<double>(scaleCoefs), widthScaled, heightScaled);
        }

		public virtual List<List<int>> GetWorkingArrangement(List<int> _data, List<double> scaleCoefs, List<int> w, List<int> h,
            List<List<int>> result = null) => throw new NotImplementedException();

        #region Вспомогательные функции 
        /// <summary>
        /// Загрузка фигур в файл пролога и выгрузка на сервер
        /// </summary>
        public void LoadFigures(List<Figure> data, List<double> scaleCoefs)
        {
            string tmpFilename = "tmpFigInfo.pl";
            FigureFileOperations.CreateNewFigFile(tmpFilename);//pathProlog + "figInfo.pl");
            FigureFileOperations.AddManyFigs(data, scaleCoefs);
            prologCluster.UploadFileToCluster(tmpFilename, "figInfo.pl");
        }

        /// <summary>
        /// Заполнение массива индексов для поиска результата
        /// </summary>
        private void ReplaceFiguresWithIndexes(List<Figure> data, out List<int> indexes)
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
        protected void FillLists(int wLast, int hLast, out List<int> w, out List<int> h, List<double> scaleCoefs)
        {
            w = new List<int>();
            h = new List<int>();
            foreach (double d in scaleCoefs)
            {
                w.Add((int)(wLast * d));
                h.Add((int)(hLast * d));
            }
        }

        protected List<int> MyCopy(List<int> list)
		{
            return new List<int>(list);
        }

        protected List<List<int>> MyCopy(List<List<int>> list)
		{
            List<List<int>> res = new List<List<int>>();
            foreach (List<int> cur in list)
                res.Add(MyCopy(cur));
            return res;
		}
        #endregion
    }
}
