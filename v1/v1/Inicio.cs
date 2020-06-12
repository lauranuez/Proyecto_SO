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
//using agario_individual;


namespace v1
{
    public partial class Inicio : Form
    {
        
        Socket server;
        Thread atender;
        string anfitrion;
        string usuario;
        string contraseña;
        string usuarioRe;
        string contraseñaRe;
        int edad;
        Consultas c;
        PartidasJugadas partJ;
        int puerto = 9200;
        string ip = "192.168.56.101";
        int timePartida;
        string chat = null;
        Random r = new Random();
        int idPartida;

        List<string> Aceptados = new List<string>();
        List<string> Respuestas = new List<string>();
        List<Partida> FormPartida = new List<Partida>();

        

        int Invitaciones;
        delegate void DelegadoParaPonerTexto(string texto);
        delegate void DelegadoParaDarseBaja();

        

        public Inicio()
        {
            InitializeComponent();
            DarseBajaBtn.Enabled = false;
            partidasJugadasToolStripMenuItem.Enabled = false;
            //Partida part = new Partida();
            //part.ShowDialog();

        }

        public void PonNoitficacion(string mensaje) //Metodo para poner la notificacion en el label
        {
            try
            {
                this.accept_invitation.Text = mensaje + " te ha invitado a jugar";
                this.accept_invitation.BackColor = Color.Yellow;
                anfitrion = mensaje;
            }
            catch (FormatException)
            {
                MessageBox.Show("No se han podido mostrar correctamente los datos");
            }
        }

        public void PonCon(string mensaje) //Metodo para poner la lista de conectados en el label
        {
            try
            {
                string[] str = mensaje.Split('-');
                int numCon = Convert.ToInt32(str[0]);
                if (numCon == 0)
                {
                    this.Conectadoslbl.Text = null;
                }
                else
                {
                    this.Conectadoslbl.Text = str[1];
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("No se han podido mostrar correctamente los datos de la lista de conectados");
            }
        } 

        public void ActGrid(string mensaje) //Metodo para poner los usuarios conectados en la tabla
        {
            try
            {
                string[] str = mensaje.Split('-');
                int numCon = Convert.ToInt32(str[0]);
                if (numCon == 0)
                {
                    dataGridView1[0, 0].Value = "No hay usuarios conectados";
                }
                else
                {
                    string usuarios = str[1];
                    string[] Users = usuarios.Split('\n');
                    dataGridView1.ColumnCount = 1;
                    dataGridView1.RowCount = Users.Length;
                    int i = 0;
                    foreach (string User in Users)
                    {
                        dataGridView1[0, i].Value = Users[i];
                        i = i + 1;
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Se ha producido un error al colocar los datos en la tabla");
            }
            
        }

        public void PonFormConectar(string m) //Metodo para activar/desactivar algunos botones/text box cuando inicias sesion
        {
            try
            {
                this.Text = m;
                DarseBajaBtn.Enabled = true;
                aceptarBtn.Enabled = false;
                usuario_tBx.Enabled = false;
                contraseña_txB.Enabled = false;
                partidasJugadasToolStripMenuItem.Enabled = true;
            }
            catch (FormatException)
            {
                MessageBox.Show("Error al poner los datos del form");
            }
        }

        public void PonFormBaja() //Metodo para activar/desactivar algunos botones/text box cuando te das de baja
        {
            try
            {
                this.Text = "Inicio";
                DarseBajaBtn.Enabled = false;
                aceptarBtn.Enabled = true;
                usuario_tBx.Enabled = true;
                contraseña_txB.Enabled = true;
            }
            catch (FormatException)
            {
                MessageBox.Show("Error al poner los datos del form");
            }
        }

        private void AtenderServidor()
        {
            try
            {
                while (true)
                {
                    int nform;
                    byte[] msg2 = new byte[80];

                    server.Receive(msg2);
                    //string m = Encoding.ASCII.GetString(msg2);
                    //MessageBox.Show(" " + m);
                    string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                    int codigo = Convert.ToInt32(trozos[0]);

                    
                    switch (codigo)
                    {
                        case 1: //Recibes la respuesta de iniciar sesion
                            try
                            {
                                string mensaj = trozos[1].Split('\0')[0];
                                if (mensaj == "1")
                                {
                                    MessageBox.Show("Usuario y contraseña correctos");
                                    this.Invoke(new DelegadoParaPonerTexto(PonFormConectar), new Object[] { usuario_tBx.Text });
                                }
                                else
                                    MessageBox.Show("Los datos introducidos no son los correctos");
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 1");
                            }

                            break;

                        case 2: //Recibes la respuesta de registrarse
                            try
                            {
                                string mensaj = trozos[1].Split('\0')[0];
                                if (mensaj == "1")
                                {
                                    MessageBox.Show("Usuario registrado correctamente");
                                }
                                else if (mensaj == "0")
                                    MessageBox.Show("Estos datos ya estan registrados");
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 2");
                            }
                            break;

                        case 3: //Recibes la respuesta de darse de baja
                            try
                            {
                                string mensaj = trozos[1].Split('\0')[0];
                                if (mensaj == "1")
                                {
                                    MessageBox.Show("Se ha dado de baja el usuario");
                                    this.Invoke(new DelegadoParaDarseBaja(PonFormBaja));
                                }
                                else
                                    MessageBox.Show("El jugador no esta en la base de datos");
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 3");
                            }
                            break;

                        case 4: //Recibes la respuesta de las consultas
                            try
                            {
                                string mensaj = trozos[1].Split('\0')[0];
                                c.TomaRespuesta(mensaj);
                                
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 4");
                            }
                            break;

                        case 5://Resultado consulta partidas
                            try
                            {
                                string mensaj = trozos[1].Split('\0')[0];
                                partJ.TomaRespuesta(mensaj);
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 13");
                            }
                            break;

                        case 6: //resultado consulta datos de partida con ese jugador
                            try
                            {
                                string mensaj = trozos[1].Split('\0')[0];
                                partJ.PonResultado(mensaj);
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 6");
                            }
                            break;

                        case 7: //Notificacion de personas conectadas
                            try
                            {
                                string mensaj = trozos[1].Split('\0')[0];
                                Conectadoslbl.Invoke(new DelegadoParaPonerTexto(PonCon), new Object[] { mensaj });
                                dataGridView1.Invoke(new DelegadoParaPonerTexto(ActGrid), new Object[] { mensaj });
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 7");
                            }
                            break;

                        case 8: //Notificacion de que ha sido invitado a jugar
                            try
                            {
                                string mensaj = trozos[1].Split('\0')[0];
                                accept_invitation.Invoke(new DelegadoParaPonerTexto(PonNoitficacion), new Object[] { mensaj });
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 8");
                            }
                            break;

                        case 9: //Mensaje de que ha aceptado la partida
                            try
                            {
                                string mensaj = trozos[1].Split('\0')[0];
                                Aceptados.Add(mensaj);
                                Respuestas.Add(mensaj);
                                MessageBox.Show(mensaj + " ha aceptado la partida");
                                if (Invitaciones == Aceptados.Count)
                                {
                                    MessageBox.Show("Todos los jugadores han aceptado la partida");
                                    Empezar_Partida();
                                }
                                else if ((Invitaciones == Respuestas.Count()) && (Respuestas.Count != Aceptados.Count()))
                                {
                                    MessageBox.Show("Algun jugador ha rechazado la partida");
                                    Invitaciones = 0;
                                    Respuestas.Clear();
                                    Aceptados.Clear();

                                }
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 9");
                            }
                            break;

                        case 10: //Notificacion de que alguien ha rechazado la partida
                            try
                            {
                                string mensaj = trozos[1].Split('\0')[0];
                                MessageBox.Show(mensaj + " ha rechazado la partida");
                                Respuestas.Add(mensaj);
                                if ((Invitaciones == Respuestas.Count()) && (Respuestas.Count != Aceptados.Count()))
                                {
                                    MessageBox.Show("Algun jugador ha rechazado la partida");
                                    Invitaciones = 0;
                                    Respuestas.Clear();
                                    Aceptados.Clear();
                                }
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 10");
                            }
                            break;

                        case 11: //Notificacion de que la partida se esta iniciando
                            try
                            {
                                MessageBox.Show("Iniciando partida");
                                ThreadStart ts = delegate { AbrirFormPartida(); };
                                Thread T = new Thread(ts);
                                T.Start();
                               
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 11");
                            }
                            break;

                        case 12: //Notificacion de que se ha enviado un mensaje al chat
                            try
                            {
                                nform = Convert.ToInt32(trozos[1]);
                                string mensaj = trozos[2].Split('\0')[0];
                                if (chat == null)
                                    chat = mensaj;
                                else
                                    chat = chat + Environment.NewLine + mensaj;
                                FormPartida[nform].ponMensaje(chat);
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 12");
                            }
                            break;

                        case 13: //Respuesta de si se han introducido bien los datos de la partida
                            break;

                        case 14:
                            string me = trozos[1].Split('\0')[0];
                            idPartida = Convert.ToInt32(me);
                            break;

                        case 15:
                            string mensaje= trozos[3].Split('\0')[0];
                            if (trozos[1] == "0")
                                FormPartida[Convert.ToInt32(trozos[2])].TomaPosicionInicial(mensaje);
                            else
                                FormPartida[Convert.ToInt32(trozos[2])].TomaPosicion(mensaje); 
                            break;

                        
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Error en la recepcion de los datos");
            }
        } //Metodo para atender los mensajes del servidor

        private void AbrirFormPartida()
        {
            try
            {
                int cont = FormPartida.Count;
                Partida p = new Partida(r,idPartida);
                p.TomaCont(cont);
                p.TomaUsuario(usuario);
                p.TomaSocket(server);
                FormPartida.Add(p);
                p.ShowDialog();
                
            }
            catch (FormatException)
            {
                MessageBox.Show("Error al abrir el form de partida");
            }
            
        } //Metodo para abrir el form de partida

        private void aceptarBtn_Click(object sender, EventArgs e)
        {
            try
            {
                usuario = usuario_tBx.Text;
                contraseña = contraseña_txB.Text;
            }
            catch (FormatException)
            {
                MessageBox.Show("Error al introducir los datos");
            }

            try
            {
                string mensaje = "1/" + usuario + "/" + contraseña;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            catch (FormatException)
            {
                MessageBox.Show("No se ha podido enviar el mensaje");
            }
        } //Metodo para enviar al servidor el mensaje de iniciar sesion con los datos del usuario

        private void enviar_Btn_Click(object sender, EventArgs e) //Metodo para enviar al servidor el mensaje de registrarse con los datos de ese usuario
        {
            int er = 0;
          
            try
            {
                usuarioRe = usuarioRe_TB.Text;
                contraseñaRe = contraseñaRe_TB.Text;
                edad = Convert.ToInt32(edadRe_TB.Text);
                usuarioRe_TB.Text = null;
                contraseñaRe_TB.Text = null;
                edadRe_TB.Text = null;
            }

            catch (FormatException)
            {
                er = 1;
                MessageBox.Show("Error de formato");
            }
            if (er == 0)
            {
                try
                {
                    string mensaje = "2/" + usuario + "/" + contraseña + "/" + edad;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                catch (FormatException)
                {
                    MessageBox.Show("No se ha podido enviar los datos");
                }
            }
        }

        private void Inicio_FormClosing(object sender, FormClosingEventArgs e) //Metodo para desconectar el usuario del servidor cuando se cierra el form
        {                        
            try
            {
                string mensaje = "0/";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                atender.Abort();
                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("No se ha desconectado correctamente");
            }
        }

        private void invite_Click(object sender, EventArgs e)
        {
            try
            {
                string invitados = null;
                bool invitacion = true;
                int Seleccionados = dataGridView1.SelectedRows.Count;
                int i = 0;
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (Convert.ToString(row.Cells[0].Value) == "No hay usuarios conectados")
                    {
                        MessageBox.Show("No hay usuarios conectados para invitar");
                        invitacion = false;
                        break;
                        
                    }
                    else if (Convert.ToString(row.Cells[0].Value) == usuario_tBx.Text)
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
                    string mensaje = "7/" + usuario_tBx.Text + "/" + invitados;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    Invitaciones = i;

                    string mensaje2 = "14/";
                    byte[] msg2 = System.Text.Encoding.ASCII.GetBytes(mensaje2);
                    server.Send(msg);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Se ha producido un error");
            }
        } //Metodo para enviar al servidor el mensaje de invitar con los nombres de los usuarios seleccionados

        private void accept_Click(object sender, EventArgs e) //Metodo para enviar al servidor que el jugador a aceptado la partida
        {
            try
            {
                string mensaje = "8/" + anfitrion + "/" + usuario_tBx.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            catch (FormatException)
            {
                MessageBox.Show("No se han podido enviar los datos");
            }
        }

        private void deny_Click(object sender, EventArgs e) //Metodo para enviar al servidor que el jugador a rechazado la partida
        {
            try
            {
                string mensaje = "9/" + anfitrion + "/" + usuario_tBx.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            catch (FormatException)
            {
                MessageBox.Show("No se han podido enviar los datos");
            }
        }

        private void Empezar_Partida() //Metodo para enviar al servidor el mensaje de empezar partida
        {
            try
            {
                string Usuarios = "";
                int i = 0;
                foreach (string usuario in Aceptados)
                {
                    Usuarios = Usuarios + usuario + ",";
                    i = i + 1;
                }
                string mensaje = "10/" + Usuarios + usuario_tBx.Text + ',';
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }

            catch (FormatException)
            {
                MessageBox.Show("No se han podido enviar los datos");
            }
        }

        private void DarseBajaBtn_Click(object sender, EventArgs e) //Metodo que se implementa para enviar los datos al servidor para darse de baja
        {
            try
            {
                usuario = usuario_tBx.Text;
                contraseña = contraseña_txB.Text;
                usuario_tBx.Text = null;
                contraseña_txB.Text = null;

                string mensaje = "3/" + usuario + "/" + contraseña;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            catch (FormatException)
            {
                MessageBox.Show("No se han podido enviar los datos");
            }
        }

        private void Inicio_Load(object sender, EventArgs e) //Metodo para conectar con el servidor cuando se abre el form
        {
            IPAddress direc = IPAddress.Parse(ip);
            IPEndPoint ipep = new IPEndPoint(direc, puerto);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
            }

            catch (SocketException)
            {
                MessageBox.Show("No se ha podido conectar con el servidor");
            }
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }

        private void consultasToolStripMenuItem_Click(object sender, EventArgs e) //Metodo que se implementa al tocar el boton de consultas para abrir ese formulario
        {
            ThreadStart ts = delegate { AbrirConsultas(); };
            Thread T = new Thread(ts);
            T.Start();
        }

        private void AbrirConsultas() //Abrimos el form de consultas
        {
            try
            {
                c = new Consultas(server);
                c.ShowDialog();
            }
            catch (FormatException)
            {
                MessageBox.Show("Error al abrir el form");
            }

        }

        private void partidasJugadasToolStripMenuItem_Click(object sender, EventArgs e) //Metodo que se implementa al tocar el boton de consultas de partidas jugadas para abrir ese formulario
        {
            ThreadStart ts = delegate { AbrirPartidas(); };
            Thread T = new Thread(ts);
            T.Start();

        }
       
        private void AbrirPartidas()//Abrir form de consulta de las partidas
        {
            try
            {
                partJ = new PartidasJugadas(server,usuario);
                partJ.ShowDialog();
            }
            catch (FormatException)
            {
                MessageBox.Show("Error al abrir el form");
            }

        }

        private void timer1_Tick(object sender, EventArgs e) //Metodo para contar los minutos de partida
        {
            timePartida = timePartida + 1;
        }

    }
}
