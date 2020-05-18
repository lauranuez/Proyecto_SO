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

namespace v1
{
    public partial class Inicio : Form
    {
        Socket server;
        Thread atender;
        List<string> Aceptados = new List<string>();
        List<string> Respuestas = new List<string>();
        int Invitaciones;
        delegate void DelegadoParaPonerTexto(string texto);
        string anfitrion;

        public Inicio()
        {
            InitializeComponent();
            IPAddress direc = IPAddress.Parse("192.168.56.101");
            IPEndPoint ipep = new IPEndPoint(direc, 9030);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                CheckForIllegalCrossThreadCalls = false;
            }

            catch (SocketException)
            {
                MessageBox.Show("No se ha podido conectar con el servidor");
                //Si hay excepcion imprimimos error y salimos del programa con return
                //Close() ;
            }
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();

        }

        public void PonNoitficacion(string mensaje)
        {
            this.accept_invitation.Text = mensaje + " te ha invitado a jugar";
            anfitrion = mensaje;

        }

        public void PonCon(string mensaje)
        {
            this.Conectadoslbl.Text = mensaje;
        }

        string usuario;
        string contraseña;
        int edad;

        private void AtenderServidor()
        {
            while (true)
            {
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);

                string mensaje = trozos[1].Split('\0')[0];
                
                switch (codigo)
                { 
                    case 1:
                                if (mensaje == "1")
                                {
                                    MessageBox.Show("Usuario y contraseña correctos");
                                }
                                else
                                    MessageBox.Show("Los datos introducidos no son los correctos");
                                break;
                    case 2:
                                if (mensaje == "1")
                                {
                                    MessageBox.Show("Usuario registrado correctamente");
                                }
                                else if (mensaje == "0")
                                    MessageBox.Show("Estos datos ya estan registrados");
                                break;
                    case 3:
                                if (mensaje == "0")
                                {
                                    MessageBox.Show("No existe esta partida");
                                }
                                else
                                    MessageBox.Show(mensaje);
                                break;
                    case 4:
                                MessageBox.Show(mensaje);
                                break;
                    case 5:
                                MessageBox.Show(mensaje);
                                break;
                    case 6:
                                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(PonCon);
                                this.Invoke(del, new Object[] { mensaje });

                                string[] str = mensaje.Split('/');
                                string usuarios = str[0];
                                string[] Users = usuarios.Split('\n');
                                
                                dataGridView1.ColumnCount = 1;
                                dataGridView1.RowCount = Users.Length;
                                int i = 0;
                                foreach (string User in Users)
                                {
                                    dataGridView1[0, i].Value = Users[i];
                                    i = i + 1;
                                }
                                break;
                    case 7:
                                del = new DelegadoParaPonerTexto(PonNoitficacion);
                                this.Invoke(del, new Object[] {mensaje});
                                accept.Enabled = true;
                                deny.Enabled = true;
                                break;
                    case 8:
                                Aceptados.Add(mensaje);
                                Respuestas.Add(mensaje);
                                MessageBox.Show(mensaje + " ha aceptado la partida");
                                if (Invitaciones == Aceptados.Count)
                                {
                                    MessageBox.Show("Todos los jugadores han aceptado la partida");
                                    Empezar_Partida();
                                }
                                else if ((Invitaciones == Respuestas.Count()) && (Respuestas.Count != Aceptados.Count()))
                                {
                                    MessageBox.Show("Algun jugador ha rechazado la partida");
                                    invite.Enabled = true;
                                    accept.Enabled = true;
                                    deny.Enabled = true;
                                    Invitaciones = 0;
                                    Respuestas.Clear();
                                    Aceptados.Clear();

                                }
                                break;
                    case 9:
                                MessageBox.Show(mensaje + " ha rechazado la partida");
                                Respuestas.Add(mensaje);
                                if ((Invitaciones == Respuestas.Count()) && (Respuestas.Count != Aceptados.Count()))
                                {
                                    MessageBox.Show("Algun jugador ha rechazado la partida");
                                    invite.Enabled = true;
                                    accept.Enabled = true;
                                    deny.Enabled = true;
                                    Invitaciones = 0;
                                    Respuestas.Clear();
                                    Aceptados.Clear();
                                }
                                break;
                    case 10:
                                MessageBox.Show("Iniciando partida");
                                break;
                }
            }
        }

        private void Empezar_Partida()
        {
            string Usuarios = "";
            int i = 0;
            foreach (string usuario in Aceptados)
            {
                Usuarios = Usuarios + usuario + ",";
                i = i + 1;
            }
            string mensaje = "9/" + Usuarios + usuario_tBx.Text + ',';
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }
        private void aceptarBtn_Click(object sender, EventArgs e)
        {
            usuario = usuario_tBx.Text;
            contraseña = contraseña_txB.Text;

            string mensaje = "1/" + usuario + "/" + contraseña;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            

        }


        private void enviar_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                usuario = usuarioRe_TB.Text;
                contraseña = contraseñaRe_TB.Text;
                edad = Convert.ToInt32(edadRe_TB.Text);
            }

            catch (FormatException)
            {
                MessageBox.Show("Error de formato");
            }


            string mensaje = "2/" + usuario + "/" + contraseña + "/" + edad;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

           

            
        }

        private void query_Click(object sender, EventArgs e)
        {
            int id_1;
            int id_2;

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
                MessageBox.Show("Error de formato");
            }

            if (NombresPartida.Checked)
            {
                string mensaje = "3/" + id1.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else if (NombresGanadores.Checked)
            {
                string mensaje = "4/";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg); 
            }
            else
            {
                string mensaje = "5/" + id2.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }


        private void Inicio_FormClosing(object sender, FormClosingEventArgs e)
        {


            usuario = usuario_tBx.Text;
            string mensaje = "0/" + usuario;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            atender.Abort();
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();

        }



        private void Inicio_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            
        }

        private void invite_Click(object sender, EventArgs e)
        {
            string invitados = null;
            bool invitacion = true;
            int Seleccionados = dataGridView1.SelectedRows.Count;
            int i = 0;
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (Convert.ToString(row.Cells[0].Value) ==  usuario_tBx.Text)
                {
                    MessageBox.Show("No puedes invitarte a ti mismo");
                    invitacion = false;
                    break;
                }
                else if (Seleccionados - 1 != i)
                {
                    invitados = invitados + row.Cells[0].Value + ",";
                }
                else
                {
                    invitados = invitados + row.Cells[0].Value;
                }
                i = i + 1;
            }
            if (invitacion == true)
            {
                string mensaje = "6/"  + usuario_tBx.Text + "/" + invitados;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                Invitaciones = i;
                invite.Enabled = false;
            }


        }

        private void accept_Click(object sender, EventArgs e)
        {
            string mensaje = "7/" + anfitrion + "/" + usuario_tBx.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            accept.Enabled = false;
            deny.Enabled = false;
            invite.Enabled = false;
        }

        private void deny_Click(object sender, EventArgs e)
        {
            string mensaje = "8/" + anfitrion + "/" + usuario_tBx.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            accept.Enabled = false;
            deny.Enabled = false;
            invite.Enabled = true;
        }
    }
}
