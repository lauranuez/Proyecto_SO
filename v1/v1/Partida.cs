using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace v1
{
    public partial class Partida : Form
    {
        string usuario;
        Socket server;
        int cont;
        int time=0;
        string ganador;
        int IDpartida;
        string fecha;
        string HoraFinal;
        string HoraInicial;
        Random r ;

        Graphics g;
        agario agar;
        agario agar2;
        ListaObst obs;
        int x = 0;
        int y = 0;
        int puntos = 0;
        int ag_vol = 20;
        int max_volume_obs = 30;
        int num_obst = 20;

        delegate void DelegadoParaPonerTexto(string texto);
        delegate void DelegadoParaPonerAgario();
        delegate void DelegadoParaPonerObstaculos();
        delegate void DelegadoParaPonerAgario2(string mensaje);

        public Partida(Random ran, int idP)
        {
            r = ran;
            IDpartida = idP;
            InitializeComponent();
            dateTimePicker1.Enabled = false;
            
        }


        public void TomaUsuario(string usuario) //Metodo para pasar del form principal el nombre de del usuario de este form
        {
            this.usuario = usuario;
            this.Text ="Partida: "+ cont+ " Usuario: "+ usuario;
        }
        
        public void TomaCont(int c) //Metodo para pasar del form principal el numero de formulario que es este
        {
            this.cont = c;
        }
        
        public void TomaSocket(Socket s) //Metodo para pasar el socket
        {
            server = s;
        }
        
        public void TomaMensaje(string m) //Metodo para poner el mensaje en el label
        {
            label1.Text = m;
            
        }
       
        public void ponMensaje(string m) //Metodo para pasar el mensaje del form principal mediante delegados
        {
            label1.Invoke(new DelegadoParaPonerTexto(TomaMensaje), new Object[] {m});
        }
       
        private void enviar_Click(object sender, EventArgs e) //Metodo para enviar el mensaje escrito por el usuario al servior para enviarlo a los demas usuarios
        {

            
        }

        public void TomaTime(string m)
        {
            timelbl.Invoke(new DelegadoParaPonerTexto(PonTime), new Object[] { time });
        }

        public void PonTime(string m) //Metodo para poner el tiempo de partida en el label
        {
            timelbl.Text = m;
        }
        
        public void PonPuntos(string m)
        {
            puntos_lbl.Text = m;
        }

        private void Partida_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            obs.graphs(g);
            agar.graph(g);
            mover();
            int i = 0;
            while (i < obs.GetNum())
            {
                if (agar.colision(obs.GetOB(i)) == 1)
                {
                    int v = obs.GetOB(i).GetV();
                    puntos = puntos + v;
                    obs.borrar(obs.GetOB(i));
                    agar.comer(pictureBox1.Width, pictureBox1.Height, v);
                    puntos_lbl.Invoke(new DelegadoParaPonerTexto(PonPuntos), new Object[] { time.ToString() });
                }
                i++;
            }

            if (obs.GetNum() == 0)
            {
                //x = 0;
                //y = 0;
                //puntos = 0;
                //agar = new agario(ag_x, ag_y, ag_vol,r);
                //obs = new ListaObst(pictureBox1.Width, pictureBox1.Height, max_volume_obs, num_obst);
                timer1.Stop();
                
                //Enviar datos de partida
                string DatoTime = dateTimePicker1.Value.ToString();
                string[] trozos = DatoTime.Split(' ');
                fecha = trozos[0];
                HoraFinal = trozos[1];
                time = time / 60;
                //MessageBox.Show("hora inicial: " + HoraInicial + "hora final: " + HoraFinal + "duracion: " + time);

                //string mensaje = "13/" + IDpartida + "/" + fecha + "/" + HoraFinal + "/" + time + "/" + ganador;
                //byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                //server.Send(msg);

                MessageBox.Show("Fin \n Ganador:");
                Close();
            }

        }
        public void mover()
        {
            if ((agar.GetX() + x + agar.GetV() <= pictureBox1.Width + agar.GetV() / 2) && (agar.GetX() + x >= 0 - agar.GetV() / 2))
            {
                agar.SetX(agar.GetX() + x);
                EnviarCambioPosicion(agar.GetX(), agar.GetY(), agar.GetV());
            }

            if ((agar.GetY() + y + agar.GetV() <= pictureBox1.Height + agar.GetV() / 2) && (agar.GetY() + y >= 0 - agar.GetV() / 2))
            {
                agar.SetY(agar.GetY() + y);
                EnviarCambioPosicion(agar.GetX(), agar.GetY(), agar.GetV());
            }
        }


        private void mensajeTB_Enter(object sender, EventArgs e)
        {
            //string mensaje = "11/" + cont + "/" + usuario + "/" + mensajeTB.Text;
            //byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            //server.Send(msg);

        }

        private void Partida_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                y = -10;
                x = 0;
            }

            if (e.KeyCode == Keys.Down)
            {
                y = 10;
                x = 0;
            }

            if (e.KeyCode == Keys.Left)
            {
                y = 0;
                x = -10;

            }
            if (e.KeyCode == Keys.Right)
            {
                y = 0;
                x = 10;
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            time = time + 1;
            timelbl.Invoke(new DelegadoParaPonerTexto(PonTime), new Object[] { time.ToString() });

        }

        private void EnviarMensaje_Click(object sender, EventArgs e)
        {

        }

        private void Partida_Load(object sender, EventArgs e)
        {
            timer1.Start();
            timer1.Interval = 1000;
            string DatoTime = dateTimePicker1.Value.ToString();
            string[] trozos = DatoTime.Split(' ');
            HoraInicial = trozos[1];

            pictureBox1.Invoke(new DelegadoParaPonerAgario(PonAgario));
            pictureBox1.Invoke(new DelegadoParaPonerObstaculos(PonObstaculo));
            EnviarPosicionInicial(agar.GetX(),agar.GetY(),agar.GetV());

            timer2.Enabled = true;
        }

        public void EnviarPosicionInicial(int x, int y, int vol)
        {

            string mensaje = "15/0/" + cont + "/" + usuario + "/" + Convert.ToString(x) + "/" + Convert.ToString(y) + "/" + Convert.ToString(vol);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }

        public void EnviarCambioPosicion(int x, int y, int vol)
        {
            string mensaje = "15/1/" + cont + "/" + usuario + "/" + Convert.ToString(x) + "/" + Convert.ToString(y) + "/" + Convert.ToString(vol);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }

        public void PonAgario()
        {

            agar = new agario(pictureBox1.Width, pictureBox1.Height, ag_vol, r);
            g = pictureBox1.CreateGraphics();
        }

        public void PonObstaculo()
        {
            obs = new ListaObst(pictureBox1.Width, pictureBox1.Height, max_volume_obs, num_obst);
            g = pictureBox1.CreateGraphics();
        }

        public void TomaPosicionInicial(string mensaje) 
        {
            pictureBox1.Invoke(new DelegadoParaPonerAgario2(PonAgario2), new Object[] { mensaje });
        }
        public void TomaPosicion(string mensaje) 
        {
            pictureBox1.Invoke(new DelegadoParaPonerAgario2(MueveAgario), new Object[] { mensaje });
        }
        public void PonAgario2(string mensaje)
        {
            string[] trozos = mensaje.Split(',');
            int x2= Convert.ToInt32(trozos[0]);
            int y2=Convert.ToInt32(trozos[1]);
            int v2= Convert.ToInt32(trozos[2]);

            agar = new agario(x2, y2, v2);
            g = pictureBox1.CreateGraphics();
        }
        public void MueveAgario(string mensaje)
        {
            string[] trozos = mensaje.Split(',');
            int x2 = Convert.ToInt32(trozos[0]);
            int y2 = Convert.ToInt32(trozos[1]);
            int v2 = Convert.ToInt32(trozos[2]);
            agar.SetX(x2);
            agar.SetY(y2);
            agar.SetV(v2);

        }

    }
}
