using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace v1
{
    class obstaculo : objeto
    {
        protected Color col = new Color();
        Random r = new Random();
        int x_pb;
        int y_pb;
        int max_vol;

        public obstaculo()
        {
            this.x = r.Next(0, x_pb);
            this.y = r.Next(0, y_pb);
            this.volumen = r.Next(0, max_vol);
            this.col = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
        }

        public void SetR(int x_pb, int y_pb, int max_vol, Random r2)
        {
            this.x_pb = x_pb;
            this.y_pb = y_pb;
            this.max_vol = max_vol;
            r = r2;
            this.x = r.Next(0, x_pb);
            this.y = r.Next(0, y_pb);
            this.volumen = r.Next(0, max_vol);
            this.col = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
        }


        public void graph(Graphics g)
        {

            g.FillEllipse(new SolidBrush(col), this.x, this.y, this.volumen, this.volumen);
        }

        public int GetV()
        {
            return volumen;
        }

        public int GetX()
        {
            return this.x;
        }
        public int GetY()
        {
            return this.y;
        }
    }
}
