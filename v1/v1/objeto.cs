using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime;

namespace v1
{
    class objeto
    {
        protected int x, y, volumen;
        public int colision(objeto o) //1 si colisionan y se lo puede comer -1 si no colisionan o si no se lo puede comer
        {
            if (volumen > o.volumen)
            {
                int r0 = volumen / 2;
                int r1 = o.volumen / 2;
                int x0 = x + r0;
                int x1 = o.x + r1;
                int y0 = y + r0;
                int y1 = o.y + r1;

                double dist = Math.Sqrt(Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2));
                if (dist <= r0)
                    return 1;
                else
                    return -1;
            }
            else
                return -1;
        }
    }
}

