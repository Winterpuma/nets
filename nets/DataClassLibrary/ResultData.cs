using System;
using System.Collections.Generic;

namespace DataClassLibrary
{
    public class ResultFigPos
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

    public class ResultData
    {
        public List<ResultFigPos> allFigures = new List<ResultFigPos>();

        public ResultData(){ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positioningData">список из строк по 3: x, y, angle, x2, y2, angle</param>
        public ResultData(System.Collections.ObjectModel.Collection<string> positioningData)
        {
            int i = 1;
            foreach (string oneFigData in positioningData)
            {
                var oneFigDataSplitted = oneFigData.Split(','); 
                string name = i.ToString();
                int xCenter = Convert.ToInt32(oneFigDataSplitted[0]);
                int yCenter = Convert.ToInt32(oneFigDataSplitted[1]);
                double angle = Convert.ToDouble(oneFigDataSplitted[2]);
                allFigures.Add(new ResultFigPos(name, xCenter, yCenter, angle));
                i++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ResultData GetRes(string result)
        {
            var res = new ResultData();
            string name;
            int xCenter, yCenter;
            double angle = 0;
            int i = 0;
            foreach (string figure in result.Split(' '))
            {
                var tmp = figure.Split(',');
                name = i.ToString();
                xCenter = Convert.ToInt32(tmp[0]);
                yCenter = Convert.ToInt32(tmp[1]);
                angle = Convert.ToDouble(tmp[2]);

                res.allFigures.Add(new ResultFigPos(name, xCenter, yCenter, angle));
                i++;
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result">f1,1,1 f2,3,1 f3,5,1 or f1_320,1,2 f2_15,0,5</param>
        /// <param name="nameWithAngle"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"> список строк вида Fig2pos,120,23</param>
        /// <param name="nameWithAngle"></param>
        public ResultData(List<string> result, bool nameWithAngle = false)
        {
            string name;
            int xCenter, yCenter;
            double angle = 0;
            foreach (string figure in result)
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
