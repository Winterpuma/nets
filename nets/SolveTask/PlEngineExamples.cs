using System;
using SbsSW.SwiPlCs.Exceptions;
using SbsSW.SwiPlCs;
using System.IO;

namespace PictureWork
{
    class PlEngineExamples
    {
        static void Ex1()
        {
            //Environment.SetEnvironmentVariable("SWI_HOME_DIR", @"the_PATH_to_boot32.prc");
            if (!PlEngine.IsInitialized)
            {
                String[] param = { "-q" };  // suppressing informational and banner messages
                PlEngine.Initialize(param);
                PlQuery.PlCall("assert(father(martin, inka))");
                PlQuery.PlCall("assert(father(uwe, gloria))");
                PlQuery.PlCall("assert(father(uwe, melanie))");
                PlQuery.PlCall("assert(father(uwe, ayala))");
                using (PlQuery q = new PlQuery("father(P, C), atomic_list_concat([P,' is_father_of ',C], L)"))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                        Console.WriteLine(v["L"].ToString());

                    Console.WriteLine("all child's from uwe:");
                    q.Variables["P"].Unify("uwe");
                    foreach (PlQueryVariables v in q.SolutionVariables)
                        Console.WriteLine(v["C"].ToString());
                }
                PlEngine.PlCleanup();
                Console.WriteLine("finshed!");
            }
        }


        public static void Ex2()
        {
            string filename = Path.GetTempFileName();
            StreamWriter sw = File.CreateText(filename);
            sw.WriteLine("father(martin, inka).");
            sw.WriteLine("father(uwe, gloria).");
            sw.WriteLine("father(uwe, melanie).");
            sw.WriteLine("father(daddy, ayala).");
            sw.Close();

            String[] param = { "-q", "-f", filename };

            try
            {
                PlEngine.Initialize(param); 

                Console.WriteLine("uwe children:");
                using (PlQuery q = new PlQuery("father(uwe, Child)"))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        Console.WriteLine(v["Child"].ToString());
                    }
                }
                Console.WriteLine("daddy children:");
                using (PlQuery q = new PlQuery("father(daddy, Child)"))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        Console.WriteLine(v["Child"].ToString());
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
        }

        public static void Ex3()
        {
            if (!PlEngine.IsInitialized)
            {
                String[] param = { "-q" };  // suppressing informational and banner messages
                PlEngine.Initialize(param);

                // Test 1
                //PlQuery.PlCall("assert(father(martin, inka))");
                //Console.WriteLine(PlQuery.PlCall("father(martin, inkal)"));

                Del myComp = DoStuff;
                PlEngine.RegisterForeign(null, "mmm", 5, myComp);

                Console.WriteLine(PlQuery.PlCall("mmm(3, 6, 9, X, Y)"));
                

                using (PlQuery q = new PlQuery("mmm(3, 6, 9, X, Y)"))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        Console.WriteLine(v["X"].ToString());
                        Console.WriteLine(v["Y"].ToString());
                    }
                }
                
                PlEngine.PlCleanup();
            }
        }

        public static bool DoStuff(PlTermV term1)
        {
            int arity = term1.Size;
            PlTerm term_X = term1[arity - 2];
            PlTerm term_Y = term1[arity - 1];

            for (int i = 0; i < arity - 2; i++)
            {
                Console.WriteLine(i.ToString() + ' ' + term1[i]);
            }
            
            term_X.Unify(new PlTerm(12));
            term_Y.Unify(new PlTerm(15));
            return true;
        }

        public delegate bool Del(PlTermV term1);
    }
}
