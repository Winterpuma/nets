using System;
using System.Collections.Generic;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;
using System.IO;

namespace PictureWork
{
    class SolutionChecker
    {
        public static List<ResultData> Tryer360(List<Figure> data)
        {
            List<ResultData> res = new List<ResultData>();
            try
            {
                string filename = "..\\..\\..\\main.pl";
                String[] param = { "-q", "-f", filename };
                PlEngine.Initialize(param);

                string queryStr = "tryer360(" +
                    QueryCreator.GetPrologAllRotatedFigureArrayRepresentationWithAngle(data) +
                    ",Res).";

                Console.WriteLine("Starting tryer360");
                //Console.WriteLine("\n\nGenerated query:\n" + queryStr);

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        string resultString = GetResultStringList(v["Res"]);
                        Console.WriteLine(resultString);
                        res.Add(GetResult(resultString, true));
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

        public static List<ResultData> F1(List<Figure> data)
        {
            List<ResultData> res = new List<ResultData>();
            try
            {
                string filename = "..\\..\\..\\main.pl";
                String[] param = { "-q", "-f", filename };
                PlEngine.Initialize(param);

                string queryStr = "f1(" + 
                    QueryCreator.GetPrologOriginalFigureArrayRepresentation(data) +
                    ",[],Res).";

                //Console.WriteLine("\n\nGenerated query:\n" + queryStr);
                Console.WriteLine("Starting prolog f1");

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        string resultString = GetResultStringList(v["Res"]);
                        Console.WriteLine(resultString);
                        res.Add(GetResult(resultString, false));
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
                Console.WriteLine("Starting prolog f1");

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

        public static ResultData GetResult(string resultString, bool nameWithAngle)
        {
            return new ResultData(resultString, nameWithAngle);
        }


        public static string GetResultStringList(PlTerm res)
        {
            return String.Join(" ", res.ToList());
        }

        public static string GetResultStringString(PlTerm res)
        {
            return String.Join(" ", res.ToString());
        }


        /*
        public static void GetOverlay()
        {
            try
            {
                string filename = "..\\..\\..\\program.pl";
                String[] param = { "-q", "-f", filename };
                PlEngine.Initialize(param);

                using (PlQuery q = new PlQuery(
                    "plane_img(Matr)," +
                    "figure_one(Deltas)," +
                    "overlay(Matr, Deltas, 3, 3, Res)."))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                        PrintMatr(v["Res"]);
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
        }

        public static void PrintMatr(PlTerm matr)
        {
            foreach (string line in matr.ToListString())
            {
                Console.WriteLine(line);
            }
        }
        */
    }
}
