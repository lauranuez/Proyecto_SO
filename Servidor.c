#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>

typedef struct{
	char usuario[20];
	int socket;
	
} Conectado;


typedef struct{
	Conectado sockets[300];
	int num;
	int numreg;
} ListaConectados;

ListaConectados UsuariosConectados;

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;


char notificacion[512];

int Add (ListaConectados *lista, int socket)
{
	
	if (lista->num == 300)
		return -1;
	else {
		lista->sockets[lista->num].socket = socket;
		lista->num++;
		return 0;
		
	}
	
}

int AddName (ListaConectados *lista, char nombre[20], int socket)
{
	
	if (lista->num == 300)
		return -1;
	else {
		int encontrado=0;
		int i=0;
		while(( i<lista->num)&& (!encontrado))
		{
			if (lista->sockets[i].socket == socket)
				encontrado=1;
			else 
				i++;
		}
		if (encontrado)
		{
			strcpy(lista->sockets[i].usuario, nombre);
			lista->numreg++;
			return 0;
		}
		else
			return -1;
		
	}
	
}

void DameConectados (ListaConectados *lista, char conectados[512])
{
	sprintf(conectados,"6/%s", lista->sockets[0].usuario);

	for(int j=1; j< lista->numreg; j++)
		sprintf(conectados,"%s\n%s", conectados, lista->sockets[j].usuario);
	
	printf("Conectados: %s\n",conectados);
}


int DesconectarUsuario(ListaConectados *lista, char nombre[20])
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
			strcpy(lista->sockets[pos].usuario,lista->sockets[pos+1].usuario);
		lista->num--;
		lista->numreg--;
		return 0;
	}
	
	else
		return -1;
		
	
	
}

int DameSocket(ListaConectados *l, char nom[20])	
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

void *AtenderCliente (void *socket)
{
		
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	conn = mysql_init(NULL);
	
	char peticion[512];
	char respuesta[512];
	int ret;
	
	if (conn==NULL) 
	{
		printf ("Error al crear la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "partidas",0, NULL, 0);
	if (conn==NULL) 
	{
		printf ("Error al inicializar la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	int terminar=0;
	while (terminar==0)
	{
		
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido\n");
		
		
		peticion[ret]='\0';
		
	
		
		printf ("Se ha conectado: %s\n",peticion);
		
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		printf("codigo= %d\n",codigo);
		
		if (codigo==0) //Desconectar Usuario
		{ 
			char usuario[20];
			p=strtok(NULL, "/");
			strcpy(usuario, p);
			pthread_mutex_lock( &mutex );
			int eliminar = DesconectarUsuario(&UsuariosConectados, usuario);
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
		
		if(codigo==1) //Conectar 
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
							int res = AddName(&UsuariosConectados,usuario,sock_conn);
							pthread_mutex_unlock(&mutex);
							sprintf(notificacion,"6/%s",UsuariosConectados.sockets[0].usuario);
							for(int j=1; j< UsuariosConectados.numreg; j++)
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
				char name[strlen(row[0])];
				strcpy(name,row[0]);
				while (row !=NULL) 
				{
					strcpy(respuesta, "2/1");
					if (strcmp (name,usuario) ==0)
					{
						strcpy(respuesta,"2/0");
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
		
		if (codigo==3) //Consulta1
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
				strcpy(respuesta,"3/Base de datos vacia");
			else
			{
				strcpy(respuesta,"3/Los jugadores son: ");
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
				if (strcmp(respuesta,"3/Los jugadores son: ")==0)
					strcpy(respuesta,"3/Esta ID no corresponde a ninguna partida (prueba 1 o 2)");
				
				
			}	
			respuesta [strlen(respuesta)-1]='\0';
		}
		
		
		
		if(codigo==4)//Consulta2
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
		
		if (codigo==5) //Consulta3
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
				strcpy(respuesta, "5/Base de datos vacia");
			else
			{
				
				char fecha[20];
				char hf[20];
				char dur[20];
				char nme[20];
				
				strcpy(fecha, row[1]);
				strcpy(hf,row[2]);
				strcpy(nme,row[4]);
				sprintf(respuesta, "5/ID: %d, Fecha: %s, Hora final: %s, Duracion: %dmin, Ganador: %s", Npartida,fecha,hf,atoi(row[3]),nme);
			}
		}
	
		if (codigo == 6) //Invitacion a jugar
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
				sprintf(notificacion,"7/%s",anfitrion);
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
		
		if (codigo == 7) //Invitacion aceptada
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
				sprintf(notificacion,"8/%s",inv);
				write (socket,notificacion, strlen(notificacion));
				printf("Notificacion: %s\n",notificacion);
			}
			else 
				printf("No se ha encontrado el socket");
		}
		
		if (codigo == 8) //Invitacion rechazada
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
				sprintf(notificacion,"9/%s",rechaza);
				write (socket,notificacion, strlen(notificacion));
				printf("Notificacion: %s\n",notificacion);
			}
			else
				printf("No se ha encontrado el socket");
		}
		if (codigo==9) //Empezar partida
		{
			char Jugador[20];
			p = strtok(NULL, ",");
			
			while (p != NULL)
			{
				strcpy(Jugador,p);
				int socket = DameSocket(&UsuariosConectados,Jugador);
				if (socket!=-1)
				{
					p = strtok(NULL, ",");
					sprintf(notificacion,"10/%s",Jugador);
					write (socket,notificacion, strlen(notificacion));
					printf("Notificacion: %s\n",notificacion);
				}
				else
					printf("No se ha encontrado el socket");
			}
			
		}
		
		if ((codigo != 0) && (codigo != 6) && (codigo !=7) && (codigo !=8) && (codigo !=9))
		{
			printf ("Respuesta: %s\n", respuesta);
			write (sock_conn,respuesta, strlen(respuesta));
		}
		
/*		if ((codigo == 0) || (codigo == 1))*/
/*		{*/
/*			int i;*/
/*			char conectados[200];*/
/*			DameConectados(&UsuariosConectados, conectados); */
/*			sprintf(notificacion,"6/%s", conectados);*/
/*			for(int j=1; j< UsuariosConectados.num; j++)*/
/*				sprintf(notificacion,"%s, %s", notificacion, UsuariosConectados.usuarios[j].usuario);*/
						
/*			for(int k=0; k<i; k++)*/
/*				write (sockets[k], notificacion, strlen(notificacion));*/
/*		}*/
		
		
		respuesta [strlen(respuesta)-1]='\0';
		
	}
	close(sock_conn);
	mysql_close (conn);
	
}

int main(int argc, char *argv[])
{	
	UsuariosConectados.num=0;
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");

	memset(&serv_adr, 0, sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	

	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);

	serv_adr.sin_port = htons(9030);
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
		Add(&UsuariosConectados,sock_conn);
		pthread_mutex_unlock(&mutex);
		
		pthread_create(&thread, NULL, AtenderCliente,&UsuariosConectados.sockets[UsuariosConectados.num-1].socket);
		
	}
}
