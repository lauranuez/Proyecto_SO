using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace v1
{
    class ListaObst : obstaculo
    {
        obstaculo[] obstaculos;
        Random r = new Random();
        int num;
        int x_pb;
        int y_pb;
        int max_vol;

        public ListaObst(int x_pb, int y_pb, int max_vol, int num_obst)
        {
            this.x_pb = x_pb;
            this.y_pb = y_pb;
            this.max_vol = max_vol;
            obstaculos = new obstaculo[num_obst];
            this.num = num_obst;
            iniciar();
        }

        public void iniciar()
        {
            for (int i = 0; i < num; i++)
            {

                obstaculos[i] = new obstaculo();
                obstaculos[i].SetR(this.x_pb, this.y_pb, this.max_vol, r);
            }
        }

        public void graphs(Graphics g)
        {
            for (int i = 0; i < num; i++)
            {
                obstaculos[i].graph(g);
            }
        }

        public obstaculo GetOB(int i)
        {
            return obstaculos[i];
        }

        public void borrar(objeto o)
        {
            int i = 0;
            bool encontrado = false;
            while ((i < num) && (!encontrado))
            {
                if (o == obstaculos[i])
                    encontrado = true;
                else
                    i++;
            }
            while (i < num - 1)
            {
                obstaculos[i] = obstaculos[i + 1];
                i++;

            }
            num--;
        }


        public int GetNum()
        {
            return this.num;
        }




    }
}
