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
        

        Graphics g;
        agario agar;
        ListaObst obs;
        int x = 0;
        int y = 0;
        int puntos = 0;
        int ag_x = 20;
        int ag_y = 20;
        int ag_vol = 20;
        int max_volume_obs = 30;
        int num_obst = 20;

        delegate void DelegadoParaPonerTexto(string texto);
        delegate void DelegadoParaCerrarFormulario();

        public Partida()
        {
            InitializeComponent();
            dateTimePicker1.Enabled = false;
            timer1.Start();
            timer1.Interval = 1000;
            string DatoTime = dateTimePicker1.Value.ToString();
            string[] trozos = DatoTime.Split(' ');
            HoraInicial = trozos[1];

            agar = new agario(ag_x, ag_y, ag_vol);
            obs = new ListaObst(pictureBox1.Width, pictureBox1.Height, max_volume_obs, num_obst);
            g = pictureBox1.CreateGraphics();
            timer2.Enabled = true;
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
                    puntos_lbl.Invoke(new DelegadoParaPonerTexto(PonPuntos), new Object[] { Convert.ToString(puntos) });
                }
                i++;
            }

            if (obs.GetNum() == 0)
            {
                x = 0;
                y = 0;
                puntos = 0;
                agar = new agario(ag_x, ag_y, ag_vol);
                obs = new ListaObst(pictureBox1.Width, pictureBox1.Height, max_volume_obs, num_obst);
                timer1.Stop();

                
                //Enviar datos de partida
                string DatoTime = dateTimePicker1.Value.ToString();
                string[] trozos = DatoTime.Split(' ');
                fecha = trozos[0];
                
                HoraFinal = DateTime.Now.ToString("HH:mm:ss");
                this.time = Convert.ToInt32(timelbl.Text);
                
                

                string mensaje = "13/" + cont + "-" + fecha + "-" + HoraFinal + "-" + time + "-" + usuario;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                
                
                
                
            }

        }

        private void AcabarPartida()
        {
            Close();
        }

        public void ganPartida(string mensaje)
        {
            this.ganador = mensaje;
            MessageBox.Show("Fin de partida \nGanador: " + ganador);
            this.Invoke(new DelegadoParaCerrarFormulario(AcabarPartida));
                
        }
        public void mover()
        {
            if ((agar.GetX() + x + agar.GetV() <= pictureBox1.Width + agar.GetV() / 2) && (agar.GetX() + x >= 0 - agar.GetV() / 2))
            {
                agar.SetX(agar.GetX() + x);
            }

            if ((agar.GetY() + y + agar.GetV() <= pictureBox1.Height + agar.GetV() / 2) && (agar.GetY() + y >= 0 - agar.GetV() / 2))
            {
                agar.SetY(agar.GetY() + y);
            }
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
                
        private void toolStripEnviar_Click(object sender, EventArgs e)
        {
            string mensaje = "11/" + cont + "/" + usuario + "/" + toolStripTextBox1.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            this.toolStripTextBox1.Text = null;
        }
    }
}
