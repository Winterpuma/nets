using System;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;

namespace PictureWork
{
    class SolutionChecker
    {
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
    }
}
