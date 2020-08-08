using System;
using System.Collections.Generic;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;
using System.IO;
using DataClassLibrary;

namespace PictureWork
{
    public static class PrologSolutionFinder
    {
        //static string tmpCodePath = "tmp_main.pl";


        private static void DbgCurLst(List<Figure> curLst)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-----------");
            curLst.ForEach((fig) =>
            {
                Console.Write(fig.name + " ");
            });
            Console.WriteLine();

            Console.ResetColor();
        }

        /*
        public static bool DoesFiguresFit(List<Figure> data, int width, int height)
        {
            DbgCurLst(data);
            string predName = "testFit";
            data.Sort(Figure.CompareFiguresBySize);
            string createdPredicate = QueryCreator.CreatePredTurnOptimized(width, height, data, predName);

            string tmpCodePath = "..\\..\\..\\tmp_main.pl";

            AppendStrToFile(tmpCodePath, prologCodePath, createdPredicate);

            bool res = false;
            try
            {
                String[] param = { "-q", "-f", tmpCodePath };
                PlEngine.Initialize(param);
                string queryStr = predName + "(Ans).";

                Console.WriteLine("Starting solution finder.");

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        var tmp = v["Ans"];
                        return true;
                    }
                    return false;
                }
            }
            catch (PlException e)
            {
                Console.WriteLine(e.MessagePl);
                Console.WriteLine(e.Message);
            }
            finally
            {
                PlEngine.PlCleanup();
            }
            return res;
        }*/


        public static bool DoesFiguresFit(int width, int height, double scale, List<int> figsIndexes)
        {
            if (GetAnyResult(width, height, scale, figsIndexes) != null)
                return true;
            else
                return false;
        }

        /*
        private static ResultData GetAnyResultTemplateAppendToExisting(List<Figure> data, int width, int height,
            Func<int, int, List<Figure>, string, string> predicateCreator,
            Func<PlTerm, string> convertResToStr,
            Func<string, ResultData> convertRes)
        {
            DbgCurLst(data);
            string predName = "testFittt";
            CreateNewMain(predicateCreator, width, height, data, predName);            
            
            try
            {
                InitEngine();
                string queryStr = predName + "(Ans).";

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        ResultData result = convertRes(convertResToStr(v["Ans"]));
                        return result;
                    }
                    return null;
                }
            }
            catch (PlException e)
            {
                Console.WriteLine(e.MessagePl);
                Console.WriteLine(e.Message);
            }
            finally
            {
                PlEngine.PlCleanup();
            }
            return null;
        }



        public static ResultData GetAnyResult(List<Figure> data, int width, int height)
        {
            return GetAnyResultTemplateAppendToExisting(data, width, height, QueryCreator.CreatePredSegmentsTurn, GetResultStringList, ResultData.GetRes);
        }*/



        private static ResultData GetAnyResultFigFile(string queryStr,
            Func<PlTerm, string> convertResToStr,
            Func<string, ResultData> convertRes)
        {

            try
            {
                if (!PlEngine.IsInitialized)
                    InitEngine();

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        ResultData result = convertRes(convertResToStr(v["Ans"]));
                        return result;
                    }
                    return null;
                }
            }
            catch (PlException e)
            {
                Console.WriteLine(e.MessagePl);
                Console.WriteLine(e.Message);
            }
            finally
            {
                PlEngine.PlCleanup();
            }
            return null;
        }
        

        
        /// <summary>
        /// Получает результат для всех фигур с масштабированием
        /// </summary>
        public static ResultData GetAnyResult(int width, int height, double scale, List<int> figInd)
        {
            // Предполагается, что файл с фигурами уже загружен
            string queryStr = QueryCreator.GetAnsQuery(width, height, scale, figInd);
            return GetAnyResultFigFile(queryStr, GetResultStringList, ResultData.GetRes);
        }

        /// <summary>
        /// Получает результат для всех фигур основываясь
        /// на другом результате (поиск в диапазоне)
        /// </summary>
        public static ResultData GetAnyResult(int width, int height, double scale, ResultData prevScaleRes, List<int> figInd)
        {
            // Предполагается, что файл с фигурами уже загружен
            string queryStr = QueryCreator.GetAnsQuery(width, height, scale, prevScaleRes, figInd);
            return GetAnyResultFigFile(queryStr, GetResultStringList, ResultData.GetRes);
        }


        #region Вспомогательные функции
        private static void InitEngine(string prologCodePath = "D:\\GitHub\\nets\\nets\\PictureWork\\main5.pl")
        {
            String[] param = { "-q", "-f", prologCodePath };
            PlEngine.Initialize(param); // м.б. исключение CallbackOnCollectedDelegate при попытке писать в swi
        }

        /*
        private static void CreateNewMain(Func<int, int, List<Figure>, string, string> predicate, int width, int height, List<Figure> data, string predName)
        {
            string createdPredicate = predicate(width, height, data, predName);
            AppendStrToFile(tmpCodePath, prologCodePath, createdPredicate);
        }*/

        private static void AppendStrToFile(string tmpCodePath, string prologCodePath, string strToAppend)
        {
            if (File.Exists(tmpCodePath))
                File.Delete(tmpCodePath);
            File.Copy(prologCodePath, tmpCodePath);
            File.AppendAllText(tmpCodePath, "\n");
            File.AppendAllText(tmpCodePath, strToAppend);
        }
        #endregion


        #region Обработка результата
        private static List<ResultData> GetFindallRes(PlTerm res)
        {
            List<ResultData> convertedRes = new List<ResultData>();
            var allResults = res.ToList();
            foreach (PlTerm curAns in allResults)
            {
                convertedRes.Add(new ResultData(curAns.ToListString()));

            }
            return convertedRes;
        }

        private static ResultData GetResult(string resultString, bool nameWithAngle)
        {
            return new ResultData(resultString, nameWithAngle);
        }


        private static string GetResultStringList(PlTerm res)
        {
            return String.Join(" ", res.ToList());
        }

        private static string GetResultStringString(PlTerm res)
        {
            return String.Join(" ", res.ToString());
        }
        #endregion
    }
}
