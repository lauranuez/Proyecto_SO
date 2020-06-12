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
    public partial class PartidasJugadas : Form
    {
        Socket server;
        string jugador;
        string usuario;
        delegate void DelegadoParaPonerTexto(string texto);

        public PartidasJugadas(Socket server, string usuario)
        {
            this.server = server;
            this.usuario = usuario;
            InitializeComponent();
        }

        private void query_Click(object sender, EventArgs e) //Enviar el mensaje al servidor segun que opcion has elegido
        {
            int err = 0;

            try
            {
                if (ResultadosPartida.Checked)
                    jugador = Jugador.Text;
            }
            catch
            {
                err = 1;
                MessageBox.Show("Error de formato");
            }
            if (err == 0) //Si se ha podido procesar los datos correctamente enviamos los mensajes de las consultas
            {
                try
                {
                    if (JugadoresPartida.Checked) //Consultar los jugadores con los que has participado en alguna partida
                    {
                        string mensaje = "12/1/" + usuario ;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                    else if (ResultadosPartida.Checked) //consultar los resultados de las partidas q has jugado con ese jugador
                    {
                        string mensaje = "12/2/"+usuario+"/"+jugador;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                    //else
                    //{
                    //    string mensaje = "12/3" ;
                    //    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    //    server.Send(msg);
                    //}
                }
                catch (FormatException)
                {
                    MessageBox.Show("No se ha podido enviar los datos");
                }
            }
        }

        public void TomaRespuesta(string m) //Poner la respuesta de la consulta 1
        {
            MessageBox.Show(m);
        }

        public void PonResultado(string m) //Poner la respuesta mediante delegados
        {
            Conectadoslbl.Invoke(new DelegadoParaPonerTexto(TomaMensaje), new Object[] { m });
        }

        public void TomaMensaje(string mensaje) //Metodo para poner la lista de conectados en el label
        {
            try
            {
                //if (mensaje == "1")
                //{
                    //this.Conectadoslbl.Text = "No has jugado ninguna partida con ese jugador";
                //}
                //else
                //{
                    this.Conectadoslbl.Text = mensaje;
                //}
            }
            catch (FormatException)
            {
                MessageBox.Show("No se han podido mostrar correctamente los datos de la lista de conectados");
            }
        } 


    }
}
