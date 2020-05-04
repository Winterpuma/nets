using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PictureWork
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles("../../../../src");
            List<Figure> data = new List<Figure>();

            foreach (string f in files)
            {
                data.Add(new Figure(f));
            }

        }
    }
}
