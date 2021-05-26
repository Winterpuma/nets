using System;
using System.Collections.Generic;
using DataClassLibrary;
using SolveTask.ServerCodeGenerators;

namespace SolveTask.Server
{
	class ServerCluster
	{
		private readonly List<IServer> cluster;

		public ServerCluster(List<IServer> cluster)
		{
            this.cluster = cluster;
		}

        /// <summary>
        /// По умолчанию адреса трактуются как пролог сервера
        /// </summary>
        /// <param name="serverAdresses">Список адресов</param>
        public ServerCluster(IEnumerable<string> serverAdresses)
        {
            cluster = new List<IServer>();

            foreach (string adress in serverAdresses)
			{
                cluster.Add(new PrologServer(adress)); // by default
			}
        }

        public void AddServer(IServer server)
		{
            cluster.Add(server);
		}

        /// <summary>
        /// Получение ответа для одного размера с нуля
        /// </summary>
        public ResultData GetAnyResult(int width, int height, double scale, List<int> figInd, int iServer = 0)
        {
            Console.WriteLine("~~~~~~ SCALE: " + scale);
            // Предполагается, что файл с фигурами уже загружен
            string queryStr = QueryCreator.GetAnsQuery(width, height, scale, figInd);
            return cluster[iServer].GetQueryResult(queryStr);
        }

        /// <summary>
        /// Получение ответа для одного размера на основе предыдущего размещения в другом масштабе
        /// </summary>
        public ResultData GetAnyResult(int width, int height, double scale, ResultData prevScaleRes, List<int> figInd, int iServer = 0)
        {
            Console.WriteLine("~~~~~~ SCALE: " + scale);
            // Предполагается, что файл с фигурами уже загружен
            string queryStr = QueryCreator.GetAnsQuery(width, height, scale, prevScaleRes, figInd);
            Console.WriteLine(queryStr);
            return cluster[iServer].GetQueryResult(queryStr);
        }

        /// <summary>
        /// Получение ответа, основанного на последовательной итерации размеров фигур внутри пролога
        /// </summary>
        public ResultData GetAnyResult(List<int> width, List<int> height, List<double> scales, List<int> figInd, int iServer = 0)
        {
            // Предполагается, что файл с фигурами уже загружен
            string queryStr = QueryCreator.GetAnsQuery(width, height, scales, figInd);
            return cluster[iServer].GetQueryResult(queryStr);
        }

        /// <summary>
        /// Загрузка файла на все сервера кластера
        /// </summary>
        /// <param name="srcFilename">Загружаемый файл</param>
        /// <param name="dstFilename">Название файла на сервере</param>
        public void UploadFileToCluster(string srcFilename, string dstFilename)
        {
            foreach (PrologServer server in cluster)
			{
                server.UploadFile(srcFilename, dstFilename);
			}
        }
    }
}
