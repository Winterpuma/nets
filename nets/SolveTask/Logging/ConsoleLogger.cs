using DataClassLibrary;
using System;
using System.Collections.Generic;

namespace SolveTask.Logging
{
	class ConsoleLogger : Logger
	{
        public void Log(List<List<int>> curSequence, ConsoleColor col = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = col;
            Console.WriteLine($"-----{TimeStamp}-----");
            curSequence.ForEach((item) =>
            {
                item.ForEach((fig) =>
                {
                    Console.Write(fig + " ");
                });
                Console.WriteLine();
            });
            Console.WriteLine();

            Console.ResetColor();
        }

        public void Log(List<int> curSequence, ConsoleColor col = ConsoleColor.Magenta)
        {
            Console.ForegroundColor = col;
            Console.WriteLine($"-----{TimeStamp}-----\nПроверяем лист: ");
            curSequence.ForEach((fig) =>
            {
                Console.Write(fig + " ");
            });
            Console.WriteLine();

            Console.ResetColor();
        }

        public void Log(ResultData data)
		{
            Console.WriteLine(data);
        }

        public void Log(string data)
        {
            Console.WriteLine(GetMsgWithTimeStamp(data));
        }

        public void LogError(string data)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(GetMsgWithTimeStamp(data));
            Console.ResetColor();
        }
    }
}
