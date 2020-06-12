using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace v1
{
    class agario : objeto
    {
        Random r = new Random();
        protected Color col = new Color();
        public agario(int x, int y, int volumen, Random r)
        {
            this.x = r.Next(0, x);
            this.y = r.Next(0, y);
            this.volumen = volumen;
            col = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
        }
        public agario(int x, int y, int volumen)
        {
            this.x = x;
            this.y = y;
            this.volumen = volumen;
            col = Color.Black;

        }

        //Color.Blue

        public void graph(Graphics g)
        {
            g.FillEllipse(new SolidBrush(col), this.x, this.y, this.volumen, this.volumen);
        }

        public void SetX(int x)
        {
            this.x = x;
        }



        public void SetY(int y)
        {
            this.y = y;
        }
        public void SetV(int v)
        {
            this.volumen = v;
        }

        public void comer(int x_pb, int y_pb, int v)
        {
            this.volumen = this.volumen + v;
            if (x + volumen >= x_pb + volumen / 2)
            {
                this.x = x_pb - volumen / 2;
            }
            if (y + volumen >= y_pb + volumen / 2)
            {
                this.y = y_pb - volumen / 2;
            }

        }


        public int GetX()
        {
            return this.x;
        }

        public int GetY()
        {
            return this.y;
        }
        public int GetV()
        {
            return this.volumen;
        }
    

    }
}
