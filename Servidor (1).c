#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>
#include <ctype.h>

typedef struct{
	char usuario[20];
	int socket;
} Conectado;

typedef struct{
	Conectado sockets[300];
	int num;
} ListaConectados;

typedef struct{
	char mensaje[100];
}Mensaje;

typedef struct{
	int num;
	Mensaje mensajes[800];
}ListaChat;

typedef struct{
	int ID; 
	char fecha[20];
	char hora_final[20];
	int min;
	char ganador[20];
}Partida;

typedef struct{
	int num;
	Partida partidas[300];
}ListaPartidas;

ListaConectados UsuariosConectados;
ListaConectados UsuariosPartida;
ListaConectados SocketConectado;
ListaChat Chat;
ListaPartidas listaPart;

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;


char notificacion[512];

int AddChat (ListaChat *lista, char m[100]) //Añadir un mensaje a la lista del chat
{
	
	if (lista->num == 800)
		return -1;
	else {
		strcpy(lista->mensajes[lista->num].mensaje,m);
		
		lista->num++;
		return 0;
		
	}
}

int Add (ListaConectados *lista, int socket, char nombre[20]) //Añadir un usuario a la lista de conectados
{
	
	if (lista->num == 300)
		return -1;
	else {
		lista->sockets[lista->num].socket=socket;
		strcpy(lista->sockets[lista->num].usuario,nombre);
		lista->num++;
		return 0;
		
	}
}
int AddPartida (ListaPartidas *lista,int ID,char fecha[20], char horafi[20],int min,char ganador[20] ) //Añadir una partida a la lista
{
	
	if (lista->num == 300)
		return -1;
	else {
		lista->partidas[lista->num].ID=ID;
		strcpy(lista->partidas[lista->num].fecha,fecha);
		strcpy(lista->partidas[lista->num].hora_final,horafi);
		lista->partidas[lista->num].min=min;
		strcpy(lista->partidas[lista->num].ganador,ganador);
		lista->num++;
		return 0;
		
	}
}

void DameConectados (ListaConectados *lista, char conectados[512]) //Devuelve los usuarios conectados
{
	sprintf(conectados,"7/%d-%s",lista->num, lista->sockets[0].usuario);

	for(int j=1; j< lista->num; j++)
		sprintf(conectados,"%s\n%s", conectados, lista->sockets[j].usuario);
	
	printf("Conectados: %s\n",conectados);
}


int DesconectarUsuario(ListaConectados *lista, char nombre[20]) //Elimina un usuario de la lista
{
	int j=0;
	int encontrado=0;
	while((j<lista->num) && !encontrado){
		if (strcmp(lista->sockets[j].usuario,nombre)==0)
			encontrado=1;
		if(!encontrado)
			j=j+1;
	}
	if (encontrado)
	{
		for(int pos=j; pos<lista->num-1; pos++)
		{
			strcpy(lista->sockets[pos].usuario,lista->sockets[pos+1].usuario);
			lista->sockets[pos].socket=lista->sockets[pos+1].socket;
		}
		lista->num--;
		return 0;
	}
	
	else
		return -1;
		
}

int DameSocket(ListaConectados *l, char nom[20]) //Devuelve el socket de ese usuario	
{
	int found = 0;
	int i = 0;
	while ((found==0) & (i < l->num))
	{
		if (strcmp(l->sockets[i].usuario,nom) == 0)
		{
			found = 1;
		}
		else
			i = i + 1;
	}
	if (found==1)
		return l->sockets[i].socket;
	else
		return -1;
}
int DameNombre(ListaConectados *l, int socket, char nombre[20]) //Devuelve el nombre del usuario con ese socket	
{
	int found = 0;
	int i = 0;
	while ((found==0) & (i < l->num))
	{
		if (l->sockets[i].socket==socket)
		{
			found = 1;
		}
		else
			i = i + 1;
	}
	if (found==1)
	{
		strcpy(nombre,l->sockets[i].usuario);
		return 1;
	}
	else
		return -1;
}

void *AtenderCliente (void *socket) //Funcion para atender al cliente
{
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [500];
	conn = mysql_init(NULL);
	
	char peticion[512];
	char respuesta[512];
	int ret;
	
	if (conn==NULL) 
	{
		printf ("Error al crear la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//Conectamos con la base de datos
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "partidas",0, NULL, 0);
	if (conn==NULL) 
	{
		printf ("Error al inicializar la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//Bucle para atender al cliente
	int terminar=0;
	while (terminar==0)
	{
		
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido\n");
		
		peticion[ret]='\0';
		
		printf ("Se ha conectado: %s\n",peticion);
		
		char *p = strtok( peticion, "/"); //Sacamos el codigo de la peticion del cliente
		int codigo =  atoi (p);
		printf("codigo= %d\n",codigo);
		
		if (codigo==0) //Desconectar Usuario
		{ 
			char usuario[20];
			DameNombre(&UsuariosConectados,sock_conn,usuario);			
			
			pthread_mutex_lock( &mutex );
			int eliminar = DesconectarUsuario(&UsuariosConectados, usuario);
			int eliminar_partida = DesconectarUsuario(&UsuariosPartida, usuario);
			pthread_mutex_unlock( &mutex );
			
			char conectados[300];
			DameConectados(&UsuariosConectados, conectados); 
			sprintf(notificacion,"%s", conectados);
			for(int k=0; k<UsuariosConectados.num; k++)
				write (UsuariosConectados.sockets[k].socket, notificacion, strlen(notificacion));
			if (eliminar==0)
				printf("El usuario se ha eliminado de la lista de conectados\n");
			else
				printf("El usuario no se ha podido eliminar de la lista de conectados\n");
			terminar=1;
		}
		
		if(codigo==1) //Iniciar sesion
		{
			char usuario[20];
			char password[20];
			p=strtok(NULL, "/");
			strcpy(usuario, p);
			p=strtok(NULL, "/");
			strcpy(password, p);
			
			err=mysql_query (conn, "SELECT * FROM datos_jugador");
			if (err!=0) 
			{
				printf ("Error al consultar datos de la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				printf ("No se han obtenido datos en la consulta\n");
			else
			{
				while (row != NULL) 
				{
					if (strcmp (row[0],usuario) ==0)
					{
						if(strcmp(row[1],password)==0)
						{
							strcpy(respuesta,"1/1");
							
							pthread_mutex_lock( &mutex ); 
							int res= Add(&UsuariosConectados,sock_conn,usuario);
							//int res = AddName(&UsuariosConectados,usuario,sock_conn);
							pthread_mutex_unlock(&mutex);
							sprintf(notificacion,"7/%d-%s",UsuariosConectados.num,UsuariosConectados.sockets[0].usuario);
							for(int j=1; j< UsuariosConectados.num; j++)
								sprintf(notificacion,"%s\n%s", notificacion, UsuariosConectados.sockets[j].usuario);
							for(int k=0; k<UsuariosConectados.num; k++)
								write (UsuariosConectados.sockets[k].socket, notificacion, strlen(notificacion));
							if (res==0)
							{
								printf("Notificacion: %s\n",notificacion);
								printf("El usuario %s se ha añadido a la lista de conectados \n", usuario );
							}
							else
								printf("La lista de usuarios esta llena");
					
						}
						else 
						{
							strcpy(respuesta,"1/0");
						}
						row = NULL;
					}
					else 
					{
						row = mysql_fetch_row (resultado);
						strcpy(respuesta,"1/0");
					}	
				}
			}
						
		}
		

		
		if (codigo==2) //Registrar
		{
			char usuario[20];
			char password[20];
			int edad;
			p=strtok(NULL, "/");
			strcpy(usuario, p);
			p=strtok(NULL, "/");
			strcpy(password, p);
			p=strtok(NULL, "/");
			edad= atoi(p);
			
			err=mysql_query (conn, "SELECT * FROM datos_jugador");
			if (err!=0) 
			{
				printf ("Error al consultar datos de la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				printf ("No se han obtenido datos en la consulta\n");
			else
			{
				int encontrado=0;
				while ((row !=NULL)&&(!encontrado)) 
				{
					char name[strlen(row[0])];
					strcpy(name,row[0]);
					strcpy(respuesta, "2/1");
					
					for (int k=0;k<strlen(name);k++)
					{
						name[k] = toupper(name[k]);
					}
					char user_mayu[20];
					for (int k=0;k<strlen(usuario);k++)
					{
						user_mayu[k] = toupper(usuario[k]);
					}
					
					if (strcmp (name,user_mayu) ==0)
					{
						strcpy(respuesta,"2/0");
						encontrado=1;
					}
					else row = mysql_fetch_row (resultado);
				}
			}
			if (strcmp(respuesta,"2/1")==0)
			{
				char edads[3];
				strcpy(consulta, "INSERT INTO datos_jugador VALUES('");
				strcat(consulta, usuario);
				strcat (consulta, "','");
				strcat(consulta, password);
				strcat (consulta, "',");
				sprintf(edads, "%d", edad);
				strcat (consulta, edads);
				strcat(consulta, ");");
				
				err=mysql_query(conn, consulta);
				if (err!=0)
				{
					printf ("Error al introducir datos de la base %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
			}
			
		}
		
		if(codigo==3) //Darse de Baja 
		{
			char usuario[20];
			char password[20];
			int found = 0;
			p=strtok(NULL, "/");
			strcpy(usuario, p);
			p=strtok(NULL, "/");
			strcpy(password, p);
			
			err=mysql_query (conn, "SELECT * FROM datos_jugador");
			if (err!=0) 
			{
				printf ("Error al consultar datos de la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				printf ("No se han obtenido datos en la consulta\n");
			else
			{
				while ((row != NULL) && (found==0))
				{
					if (strcmp (row[0],usuario) ==0)
					{
						if(strcmp(row[1],password)==0)
						{
							found=1;
						}
					}
					else 
					{
						row = mysql_fetch_row (resultado);
					}
				}
				if (found==1)
				{
					strcpy(consulta, "DELETE FROM datos_jugador WHERE (username='");
					strcat(consulta, usuario);
					strcat (consulta, "' AND pass='");
					strcat(consulta, password);
					strcat (consulta, "');");
					err=mysql_query(conn, consulta);
					if (err!=0)
					{
						printf ("Error al introducir datos de la base %u %s \n", mysql_errno(conn), mysql_error(conn));
						exit(1);
					}
					strcpy(respuesta,"3/1");
					
					int s=DameSocket(&UsuariosConectados,usuario);
					
					pthread_mutex_lock( &mutex ); 
					DesconectarUsuario(&UsuariosConectados,usuario);
					pthread_mutex_unlock(&mutex);
					
					sprintf(notificacion,"7/%d-%s",UsuariosConectados.num,UsuariosConectados.sockets[0].usuario);
					write (s, notificacion, strlen(notificacion));
					
					for(int j=1; j< UsuariosConectados.num; j++)
						sprintf(notificacion,"%s\n%s", notificacion, UsuariosConectados.sockets[j].usuario);
					for(int k=0; k<UsuariosConectados.num; k++)
						write (UsuariosConectados.sockets[k].socket, notificacion, strlen(notificacion));
					printf("Notificacion: %s\n",notificacion);
					
					
				}
				else
				{
					strcpy(respuesta,"3/0");
				}
			}
			
		}
		
		
		if (codigo==4) //Consulta1: Muestra los nombres de los jugadores de esa partida
		{
			int Npartida;
			p=strtok(NULL, "/");
			Npartida=atoi(p);
			
			strcpy(consulta,"SELECT * FROM partida");
			err=mysql_query(conn,consulta);
			
			if (err!=0)
			{
				printf("Error al realizar la consulta %u %s \n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			resultado=mysql_store_result(conn);
			row = mysql_fetch_row(resultado);
			
			if (row == NULL)
				strcpy(respuesta,"4/Base de datos vacia");
			else
			{
				strcpy(respuesta,"4/Los jugadores son: ");
				while (row!=NULL)
				{
					int cont = atoi(row[1]);
					if (cont==Npartida)
					{
						strcat (respuesta, row[0]);
						strcat (respuesta, " ");
					}
					row = mysql_fetch_row(resultado);
				}
				if (strcmp(respuesta,"4/Los jugadores son: ")==0)
					strcpy(respuesta,"4/Esta ID no corresponde a ninguna partida (prueba 1 o 2)");
				
				
			}	
			respuesta [strlen(respuesta)-1]='\0';
		}
		
		
		
		if(codigo==5)//Consulta2: Muestra todos los ganadores
		{
			err=mysql_query (conn, "SELECT * FROM datos_partida");
			if (err!=0) 
			{
				printf ("Error al consultar datos de la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				strcpy(respuesta, "4/Base de datos vacia");
			else
			{
				strcpy (respuesta, "4/Los ganadores son :");
				while (row !=NULL) 
				{
					strcat (respuesta, row[4]); 
					strcat (respuesta, " ");
					row = mysql_fetch_row (resultado);
				}
				
			}
			
			respuesta [strlen(respuesta)-1]='\0';
			
			
		}
		
		if (codigo==6) //Consulta3: da los datos de esa partida
		{
			int Npartida;
			p=strtok(NULL, "/");
			Npartida=atoi(p);
			char np[20];
			
			
			strcpy (consulta,"SELECT * FROM datos_partida WHERE ID = " );
			sprintf(np, "%d", Npartida);
			strcat (consulta, np);
			strcat (consulta, ";");
			err=mysql_query (conn,consulta );
			if (err!=0) 
			{
				printf ("Error al consultar datos de la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				strcpy(respuesta, "4/Base de datos vacia");
			else
			{
				
				char fecha[20];
				char hf[20];
				char dur[20];
				char nme[20];
				
				strcpy(fecha, row[1]);
				strcpy(hf,row[2]);
				strcpy(nme,row[4]);
				sprintf(respuesta, "4/ID: %d, Fecha: %s, Hora final: %s, Duracion: %d, Ganador: %s", Npartida,fecha,hf,atoi(row[3]),nme);
			}
		}
	
		if (codigo == 7) //Invitacion a jugar
		{   
			p = strtok(NULL, "/");
			char anfitrion[20];
			strcpy(anfitrion,p);
			p = strtok(NULL, "/");
			
			
			while (p!=NULL)
			{
				char Invitado[20];
				strcpy(Invitado,p);
				int socket = DameSocket(&UsuariosConectados,Invitado);
				sprintf(notificacion,"8/%s",anfitrion);
				if (socket!=-1)
				{
					write (socket,notificacion, strlen(notificacion));
					printf("Notificacion: %s\n",notificacion);
				}
				else
					printf("No se ha encontrado el socket\n");
				p = strtok(NULL, ",");
			}
		}
		
		if (codigo == 8) //Invitacion de juego aceptada
		{
			char anf[20];
			char inv[20];
			p = strtok(NULL, "/");
			strcpy(anf,p);
			p = strtok(NULL, "/");
			strcpy(inv,p);
			int socket = DameSocket(&UsuariosConectados,anf);
			if (socket != -1)
			{
				sprintf(notificacion,"9/%s",inv);
				write (socket,notificacion, strlen(notificacion));
				printf("Notificacion: %s\n",notificacion);
			}
			else 
				printf("No se ha encontrado el socket");
		}
		
		if (codigo == 9) //Invitacion de juego rechazada
		{
			char anfi[20];
			char rechaza[20];
			p = strtok(NULL, "/");
			strcpy(anfi,p);
			p = strtok(NULL, "/");
			strcpy(rechaza,p);
			int socket = DameSocket(&UsuariosConectados,anfi);
			if (socket !=-1)
			{
				sprintf(notificacion,"10/%s",rechaza);
				write (socket,notificacion, strlen(notificacion));
				printf("Notificacion: %s\n",notificacion);
			}
			else
				printf("No se ha encontrado el socket");
		}
		if (codigo==10) //Empezar partida
		{
			char Jugador[20];
			p = strtok(NULL, ",");
			
			while (p != NULL)
			{
				strcpy(Jugador,p);
				int socket = DameSocket(&UsuariosConectados,Jugador);
				Add(&UsuariosPartida,socket,Jugador);
				
				if (socket!=-1)
				{
					p = strtok(NULL, ",");
					sprintf(notificacion,"11/%s",Jugador);
					write (socket,notificacion, strlen(notificacion));
					printf("Notificacion: %s\n",notificacion);
				}
				else
					printf("No se ha encontrado el socket");
			}
		}
		
		if (codigo==11) //Mensaje chat
		{
			char Jugador[20];
			char mensajeJugador[20];
			char mensaje[20];
			int numForm;
			p = strtok(NULL, "/");
			numForm=atoi(p);
			p = strtok(NULL, "/");
			strcpy(Jugador,p);
			p = strtok(NULL, "/");
			strcpy(mensajeJugador,p);
			sprintf(mensaje,"%s:    %s",Jugador,mensajeJugador);
			
			sprintf(notificacion, "12/%d/%s", numForm, mensaje);			
			
			for(int i=0; i<UsuariosPartida.num; i++)
			{
				printf("Jugador:%s Socket:%d \n", UsuariosPartida.sockets[i].usuario,UsuariosPartida.sockets[i].socket);
				write (UsuariosPartida.sockets[i].socket,notificacion, strlen(notificacion));
				printf("Notificacion: %s\n",notificacion);
			}
			
						
		}
		if (codigo ==12) //Consultas sobre las partidas
		{
			int numCons;
			char usuario[20];
			p = strtok(NULL, "/");
			numCons=atoi(p);
			
			if (numCons==1) //Jugadores con los que he hechado alguna partida
			{
				p = strtok(NULL, "/");
				strcpy(usuario,p);
				
				sprintf(consulta,"SELECT jugador FROM partida where ID_partida IN (SELECT ID_partida FROM partida where jugador='%s')",usuario);
				err=mysql_query(conn,consulta);
				
				if (err!=0)
				{
					printf("Error al realizar la consulta %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
				resultado=mysql_store_result(conn);
				row = mysql_fetch_row(resultado);
				
				if (row == NULL)
					strcpy(respuesta,"5/Base de datos vacia ");
				else
				{
					while (row!=NULL)
					{
						strcpy(respuesta,"5/Los jugadores son: ");
						char nombre[20]; 
						strcpy(nombre,row[0]);
						if (strcmp(usuario, nombre)!=0)
						{
							sprintf(respuesta,"%s%s,",respuesta, nombre);
						}
						row = mysql_fetch_row(resultado);
					}
					
					
					
				}	
				respuesta [strlen(respuesta)-1]='\0';
			}
				
			
			if (numCons==2) //Resultado de las partidas jugadas con ese jugador
			{
				char jugadorPar[20];
				char gan[20];
				int numP;
				
				p = strtok(NULL, "/");
				strcpy(usuario,p);
				p = strtok(NULL, "/");
				strcpy(jugadorPar,p);
				sprintf(consulta,"select datos_partida.ganador, datos_partida.ID, datos_partida.fecha, datos_partida.hora_final, datos_partida.duracion_min from partida, datos_partida where partida.jugador='%s' and partida.ID_partida in (select partida.ID_partida from partida where partida.jugador='%s') and partida.ID_partida=datos_partida.ID;",usuario,jugadorPar);
				err=mysql_query(conn,consulta);
				
				if (err!=0)
				{
					printf("Error al realizar la consulta %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
				resultado=mysql_store_result(conn);
				row = mysql_fetch_row(resultado);
				
				if (row == NULL)
					strcpy(respuesta,"6/No has jugado ninguna partida con ese jugador. ");
				else
				{
					strcpy(gan,row[0]);
					numP=atoi(row[1]);
					sprintf(respuesta,"6/Partida: %d, Ganador: %s, Fecha: %s,  Duracion: %s min ",numP, gan, row[2],row[4]);
					row = mysql_fetch_row(resultado);
					while (row!=NULL)
					{
						strcpy(gan,row[0]);
						numP=atoi(row[1]);
						sprintf(respuesta,"%s\n Partida: %d, Ganador: %s, Fecha: %s, Duracion: %s min ",respuesta, numP, gan, row[2],row[4]);
						row = mysql_fetch_row(resultado);
					}
				}	
				
			}
			if(numCons==3) //Partidas jugadas en una fecha concreta
			{
				int dia;
				int mes;
				int anyo;
				char fecha[20];
				p = strtok(NULL, "/");
				dia=atoi(p);
				p = strtok(NULL, "/");
				mes=atoi(p);
				p = strtok(NULL, "/");
				anyo=atoi(p);
				sprintf(fecha,"%d-%d-%d", anyo, mes, dia);
				
				sprintf(consulta, "select datos_partida.ID from datos_partida where datos_partida.fecha = '%s';", fecha);
				err=mysql_query(conn,consulta);
				
				if (err!=0)
				{
					printf("Error al realizar la consulta %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
				resultado=mysql_store_result(conn);
				row = mysql_fetch_row(resultado);
				
				if (row == NULL)
					strcpy(respuesta,"15/No hay partidas jugadas en esa fecha.");
				else
				{
					strcpy(respuesta, "15/Partidas jugadas:");
					while(row!=NULL)
					{
						sprintf(respuesta, "%s %s,",respuesta, row[0]);
						row = mysql_fetch_row(resultado);
					}
				}
				
			}
			
		}
		
		if (codigo==13) //Introducir partida en la base de datos
		{
			
			char fecha[20];
			int dia;
			int mes;
			int anyo;
			char hora_final[20];
			int min;
			char ganador[20];
			int numF;
			
			p=strtok(NULL, "-");
			numF= atoi(p);
			p=strtok(NULL, "/");
			dia=atoi(p);
			p=strtok(NULL, "/");
			mes=atoi(p);
			p=strtok(NULL, "-");
			anyo=atoi(p);
			sprintf(fecha,"%d-%d-%d", anyo, mes, dia);
			p=strtok(NULL, "-");
			strcpy(hora_final, p);
			p=strtok(NULL, "-");
			min= atoi(p);
			p=strtok(NULL, "-");
			strcpy(ganador, p);
			
			sprintf(consulta, "INSERT INTO datos_partida (fecha, hora_final, duracion_min, ganador) VALUES('%s','%s',%d,'%s');", fecha, hora_final, min, ganador);
			sprintf(notificacion, "14/%d/%s", numF, ganador);			
			for(int i=0; i<UsuariosPartida.num; i++)
			{
				printf("Jugador:%s Socket:%d \n", UsuariosPartida.sockets[i].usuario,UsuariosPartida.sockets[i].socket);
				write (UsuariosPartida.sockets[i].socket,notificacion, strlen(notificacion));
				printf("Notificacion: %s\n",notificacion);
			}
			err=mysql_query(conn, consulta);
			if (err!=0)
			{
				printf ("Error al introducir datos de la base %u %s \n", mysql_errno(conn), mysql_error(conn));
				exit(1);
				strcpy(respuesta,"13/Error al introducir los datos en la base de datos ");
			}
			else
			{
				strcpy(respuesta,"13/Partida guardada correctamente ");
			}
		}
			
		
		
		
		
		
		if ((codigo != 0) && (codigo != 7) && (codigo !=8) && (codigo !=9) && (codigo !=10)&& (codigo !=11) && (codigo !=14)) //Enviar respuesta
		{
			printf ("Respuesta: %s\n", respuesta);
			write (sock_conn,respuesta, strlen(respuesta));
		}
		respuesta [strlen(respuesta)-1]='\0';
		
	}
	//Cerrar la conexion del socket y de la base de datos
	close(sock_conn);
	mysql_close (conn);
	
}

int main(int argc, char *argv[])
{	
	UsuariosConectados.num=0;
	UsuariosPartida.num=0;
	Chat.num=0;
	listaPart.num=0;
	
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");

	memset(&serv_adr, 0, sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	

	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);

	serv_adr.sin_port = htons(9100);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind\n");
	
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen");
	
	
	pthread_t thread;

	for(;;)
	{
		printf ("Escuchando\n");
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion \n");
		
		pthread_mutex_lock(&mutex); 
		Add(&SocketConectado,sock_conn," ");
		pthread_mutex_unlock(&mutex);
		
		pthread_create(&thread, NULL, AtenderCliente,&SocketConectado.sockets[SocketConectado.num-1].socket);
		
	}
}
