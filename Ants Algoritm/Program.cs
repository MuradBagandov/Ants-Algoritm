using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ants_Algoritm
{
    class Program
    {
        static AntsAlgoritm aa;

        public static List<Point> points = new List<Point>() {
        new Point(5,3),
        new Point(20,18),
        new Point(33,2),
        new Point(1,48)
        };

        static void Main(string[] args)
        {
            //aa = new AntsAlgoritm(points);
            //aa.Generate(30);
            //var trai = aa.ComputeTrajectory(0);
            using (var a = new WindowGL(800, 600))
            {
                a.Run(60, 60);
            }
        }
    }
}
