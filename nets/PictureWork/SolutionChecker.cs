using System;
using System.Collections.Generic;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;
using System.IO;

namespace PictureWork
{
    class SolutionChecker
    {
        public static List<ResultData> CreateAndRunTest(List<Figure> data, int width, int height, string prologCodePath = "..\\..\\..\\main.pl")
        {
            string predName = "testFigsNoTurn";
            string createdPredicate = QueryCreator.CreateFigPredNoTurn(width, height, data, predName);

            string tmpCodePath = "..\\..\\..\\tmp_main.pl";

            if (File.Exists(tmpCodePath))
                File.Delete(tmpCodePath);
            File.Copy(prologCodePath, tmpCodePath);
            File.AppendAllText(tmpCodePath, "\n");
            File.AppendAllText(tmpCodePath, createdPredicate);

            List<ResultData> res = new List<ResultData>();
            try
            {
                String[] param = { "-q", "-f", tmpCodePath };
                PlEngine.Initialize(param);
                var vars = QueryCreator.CreateListOfResulVars(data.Count);
                string queryStr = predName + "(" + String.Join(",", vars) + ").";

                //Console.WriteLine("\n\nGenerated query:\n" + queryStr);
                Console.WriteLine("Starting solution finder.");

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        List<string> resultStrings = new List<string>();
                        foreach (string variable  in vars)
                        {
                            string result = GetResultStringString(v[variable]);
                            Console.WriteLine(variable + "," + result);
                            resultStrings.Add(variable + "," + result);
                        }

                        res.Add(new ResultData(resultStrings, false));
                    }
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
        }

        public static List<ResultData> CreateAndRunTestTurning(List<Figure> data, int width, int height, string prologCodePath = "..\\..\\..\\main.pl")
        {
            string predName = "testFigsTurn";
            string createdPredicate = QueryCreator.CreatePredTurn(width, height, data, predName);

            string tmpCodePath = "..\\..\\..\\tmp_main.pl";

            AppendStrToFile(tmpCodePath, prologCodePath, createdPredicate);

            List<ResultData> res = new List<ResultData>();
            try
            {
                String[] param = { "-q", "-f", tmpCodePath };
                PlEngine.Initialize(param);
                string queryStr = "findall(X, " + predName + "(X), Lx).";

                //Console.WriteLine("\n\nGenerated query:\n" + queryStr);
                Console.WriteLine("Starting solution finder.");

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        res = GetFindallRes(v["Lx"]); 
                    }
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
        }

        public static List<ResultData> CreateAndRunTestTurningOptimized(List<Figure> data, int width, int height, string prologCodePath = "..\\..\\..\\main.pl")
        {
            string predName = "testFigsTurnOpt";
            string createdPredicate = QueryCreator.CreatePredTurnOptimized(width, height, data, predName);

            string tmpCodePath = "..\\..\\..\\tmp_main.pl";

            AppendStrToFile(tmpCodePath, prologCodePath, createdPredicate);

            List<ResultData> res = new List<ResultData>();
            try
            {
                String[] param = { "-q", "-f", tmpCodePath };
                PlEngine.Initialize(param);
                string queryStr = "findall(X, " + predName + "(X), Lx).";

                //Console.WriteLine("\n\nGenerated query:\n" + queryStr);
                Console.WriteLine("Starting solution finder.");

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        res = GetFindallRes(v["Lx"]);
                    }
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
        }

        public static bool DoesFiguresFit(List<Figure> data, int width, int height, string prologCodePath = "..\\..\\..\\main.pl")
        {
            string predName = "testFit";
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
                    res = q.NextSolution();
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
        }

        private static int CountDeltas(List<Figure> data)
        {
            int sum = 0;
            foreach(Figure f in data)
            {
                sum += f[0].Count;
            }
            return sum;
        }

        private static bool DoesCurrentArrangementFit(List<List<Figure>> sequence, int w, int h)
        {
            int area = w * h;
            foreach (List<Figure> curLstArrangement in sequence)
            {
                if (CountDeltas(curLstArrangement) > area || !DoesFiguresFit(curLstArrangement, w, h))
                    return false;
            }
            return true;
        }

        public static List<List<Figure>> AllPosibleArrangements(List<Figure> data, int w, int h, int MLists, List<List<Figure>> curSequence = null)
        {
            if (curSequence == null)
            {
                curSequence = new List<List<Figure>>();
                for (int i = 0; i < MLists; i++)
                    curSequence.Add(new List<Figure>());
            }
            if (data.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("-----------");
                curSequence.ForEach((item) =>
                {
                    item.ForEach((fig) =>
                    {
                        Console.Write(fig.name + " ");
                    });
                    Console.WriteLine();
                });
                Console.WriteLine();

                Console.ResetColor();

                if (DoesCurrentArrangementFit(curSequence, w, h)) // Проверка подходит ли текущий результат
                    return curSequence;
                else
                    return null;
            }

            for (int i = 0; i < MLists; i++)
            {
                List<List<Figure>> newSequence = new List<List<Figure>>();

                curSequence.ForEach((item) =>
                {
                    newSequence.Add(new List<Figure>(item));
                });

                var newData = new List<Figure>(data);
                newData.RemoveAt(0);
                newSequence[i].Add(data[0]);

                List<List<Figure>> res = AllPosibleArrangements(newData, w, h, MLists, newSequence);
                if (res != null)
                    return res;
            }
            return null;
        }

        private static bool DoesFiguresFitOnMLists(List<Figure> data, int width, int height, int MLists)
        {
            if (AllPosibleArrangements(data, width, height, MLists) != null)
                return true;
            return false;
        }

        /// <summary>
        /// Поиск расположения на минимальном кол-ве листов
        /// </summary>
        public static List<List<Figure>> FindMinArrangement(List<Figure> data, int width, int height)
        {
            int maxLstNumber = data.Count;
            int minLstNumber = 1;
            List<List<Figure>> res = new List<List<Figure>>();

            do
            {
                int curLstNumber = GetHalf(minLstNumber, maxLstNumber);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("------ min: " + minLstNumber + " max: " + maxLstNumber + " cur: " + curLstNumber);
                Console.ResetColor();
                
                var curRes = AllPosibleArrangements(data, width, height, curLstNumber);
                if (curRes != null)
                {
                    maxLstNumber = curLstNumber;
                    res = curRes;
                }
                else
                    minLstNumber = curLstNumber + 1;
            }
            while (maxLstNumber != minLstNumber);

            return res;
        }

        private static int GetHalf(int min, int max)
        {
            return min + (int)Math.Floor((double)((max - min) / 2));
        }

        private static void AppendStrToFile(string tmpCodePath, string prologCodePath, string strToAppend)
        {
            if (File.Exists(tmpCodePath))
                File.Delete(tmpCodePath);
            File.Copy(prologCodePath, tmpCodePath);
            File.AppendAllText(tmpCodePath, "\n");
            File.AppendAllText(tmpCodePath, strToAppend);
        }

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
    }
}
