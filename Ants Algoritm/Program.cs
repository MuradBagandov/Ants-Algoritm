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

        static void Main(string[] args)
        {
            using (var GLW = new WindowGL(800, 600))
            {
                GLW.Run(60, 60);
            }
        }
    }
}
