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

namespace v1
{
    public partial class Consultas : Form
    {
        Socket server;
        public Consultas(Socket server)
        {
            this.server = server;
            InitializeComponent();
        }

        private void query_Click(object sender, EventArgs e) //Enviar el mensaje al servidor segun que opcion has elegido
        {
            int id_1;
            int id_2;
            int err = 0;
            try
            {
                if (NombresPartida.Checked)
                    id_1 = Convert.ToInt32(id1.Text);

                else if (partida.Checked)
                {
                    id_2 = Convert.ToInt32(id2.Text);
                }

            }
            catch
            {
                err = 1;
                MessageBox.Show("Error de formato");
            }
            if (err == 0)
            {
                try
                {
                    if (NombresPartida.Checked)//Enviar al servidor el mensaje para saber los nombres de los jugadores de esa partida
                    {
                        string mensaje = "4/" + id1.Text;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }

                    else if (NombresGanadores.Checked) //Enviar al servidor el mensaje para saber los nombres de todos los ganadores
                    {
                        string mensaje = "5/";
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }

                    else //Mensaje para saber los datos de esa partida
                    {
                        string mensaje = "6/" + id2.Text;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("No se ha podido enviar los datos");
                }
            }
           
        }

        public void TomaRespuesta(string m) //Metodo para recibir la respuesta del servidor  del form inicial
        {
           MessageBox.Show(m);
        }

    }
}
