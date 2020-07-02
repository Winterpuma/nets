using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWork
{
    class ResultFigPos
    {
        public string name = "noname";
        public int xCenter, yCenter = 0;
        public double angle = 0;


        public ResultFigPos(string name, int xCenter, int yCenter, double angle = 0)
        {
            this.name = name;
            this.xCenter = xCenter;
            this.yCenter = yCenter;
            this.angle = angle;
        }
    }

    class ResultData
    {
        public List<ResultFigPos> allFigures = new List<ResultFigPos>();


        // f1,1,1 f2,3,1 f3,5,1 or f1_320,1,2 f2_15,0,5
        public ResultData(string result, bool nameWithAngle = false)
        {
            string name;
            int xCenter, yCenter;
            double angle = 0;
            foreach (string figure in result.Split(' '))
            {
                var tmp = figure.Split(',');
                if (nameWithAngle)
                {
                    var seperatorIndex = tmp[0].LastIndexOf('_');
                    name = tmp[0].Substring(0, seperatorIndex);
                    angle = Convert.ToDouble(tmp[0].Substring(seperatorIndex + 1));
                }
                else
                {
                    name = tmp[0];
                }

                xCenter = Convert.ToInt32(tmp[1]);
                yCenter = Convert.ToInt32(tmp[2]);

                allFigures.Add(new ResultFigPos(name, xCenter, yCenter, angle));
            }
        }

        public static List<ResultData> PackAllPossibleResults(IEnumerable<string> allResults, bool flagNameWithAngle)
        {
            List<ResultData> res = new List<ResultData>();
            foreach (string currentResult in allResults)
                res.Add(new ResultData(currentResult, flagNameWithAngle));
            return res;
        }

    }
}
